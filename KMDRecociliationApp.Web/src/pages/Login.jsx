import React, { useEffect, useState } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  InputOTP,
  InputOTPGroup,
  InputOTPSlot,
} from "@/components/ui/input-otp";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import RButton from "@/components/ui/rButton";
import { ToastAction } from "@/components/ui/toast";
import { useToast } from "@/components/ui/use-toast";
import { userStore } from "@/lib/store";
import { login, verifyOTP } from "@/services/authenticate";
import { getCurrentannouncement } from "@/services/announcement";

import { KeyRound, Loader2, Megaphone, Smartphone } from "lucide-react";
import { useNavigate } from "react-router-dom/dist";
import { clsx } from "clsx";
import { Button } from "@/components/ui/button";
import kmd from "@/assets/kmd.png";
import banner from "@/assets/bannerimage.jpg";

const Login = () => {
  const navigate = useNavigate();
  const { toast } = useToast();
  const user = userStore((state) => state.user);
  const updateUser = userStore((state) => state.updateUser);

  const [loginMethod, setLoginMethod] = useState("otp");
  const [showOTP, setShowOTP] = useState(false);
  const [credentials, setCredentials] = useState({
    phoneNumber: "",
    otp: "",
    password: "",
  });
  const [phoneError, setPhoneError] = useState("");
  const [announcementText, setannouncementText] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [countdown, setCountdown] = useState(30);
  const [canResend, setCanResend] = useState(false);
  // Get campaign ID from URL
  const campaignId = new URLSearchParams(window.location.search).get(
    "campaignId"
  );

  useEffect(() => {
    if (user) {
      let role = user?.userType?.name.toLowerCase().trim();

      // if (role !== "pensioner" && role !== "community") {
      //   user["isProfileComplete"] = true;
      // }
      // if (role === "pensioner" || role === "community") {
      //   if (user?.isPolicy === false) {
      //     if (campaignId > 0) {
      //       window.location.href = `/productlist?campaignId=${campaignId}`;
      //     } else {
      //       window.location.href = "/productlist";
      //     }
      //   } else if (user?.isProfileComplete) {
      //     if (campaignId > 0) {
      //       window.location.href = `/profile?campaignId=${campaignId}`;
      //     } else {
      //       window.location.href = "/profile";
      //     }
      //   } else {
      //     window.location.href = "/";
      //   }
      // } else {
      window.location.href = "/dashboard";
      //}
    }
    (async () => {
      await getAnnouncement();
    })();
  }, []);

  useEffect(() => {
    let timer;
    if (showOTP && countdown > 0) {
      timer = setTimeout(() => setCountdown(countdown - 1), 1000);
    } else if (countdown === 0) {
      setCanResend(true);
    }
    return () => clearTimeout(timer);
  }, [showOTP, countdown]);

  const handleChange = (name) => (event) => {
    let value = name === "otp" ? event : event.target.value;

    if (name === "phoneNumber") {
      value = value.replace(/\D/g, "").slice(0, 16);
      if (value.length === 0) {
        setPhoneError("");
      }
      // else if (value.length !== 10) {
      //   setPhoneError("Please enter a 10-digit mobile number");
      // }
      else {
        setPhoneError("");
      }
    }

    setCredentials((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const resetTimer = () => {
    setCountdown(30);
    setCanResend(false);
  };

  const handleLoginWithPassword = async () => {
    // if (credentials.phoneNumber.length !== 10) {
    //   setPhoneError("Please enter a valid  mobile number");
    //   return;
    // }

    if (!credentials.password) {
      toast({
        variant: "destructive",
        title: "Error!",
        description: "Please enter your password",
      });
      return;
    }

    setIsLoading(true);
    try {
      const response = await login({
        phoneNumber: credentials.phoneNumber,
        password: credentials.password,
        isPasswordLogin: true,
      });

      if (response.status === "success") {
        toast({
          description: "Logged in successfully.",
        });

        let user = response.data;
        let role = user?.userType?.name.toLowerCase().trim();

        if (role !== "pensioner") {
          user["isProfileComplete"] = true;
        }
        updateUser(user);

        // if (role === "pensioner" || role === "community") {
        //   if (user?.isPolicy === false) {
        //     window.location.href = "/productlist";
        //   } else if (user?.isProfileComplete) {
        //     window.location.href = "/productlist";
        //   } else {
        //     window.location.href = "/";
        //   }
        // } else {
        window.location.href = "/dashboard";
        //}
      } else {
        toast({
          variant: "destructive",
          title: "Error!",
          description: response.errors?.[0] || "Invalid credentials",
        });
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Error!",
        description: "There was a problem with your request.",
      });
    }
    setIsLoading(false);
  };

  const handleLogin = async () => {
    if (loginMethod === "password") {
      await handleLoginWithPassword();
      return;
    }

    // if (credentials.phoneNumber.length !== 10) {
    //   setPhoneError("Please enter a valid 10-digit mobile number");
    // } else

    setIsLoading(true);
    let response = await login(credentials);
    if (response.status === "success") {
      setShowOTP(true);
      resetTimer();
    } else {
      if (response.status === "unauthorized") {
        toast({
          title: "Error!",
          description: response.errors[0],
          action: (
            <ToastAction altText="Try again" onClick={handleLogin}>
              Try again
            </ToastAction>
          ),
        });
      } else if (response.status === "notFound") {
        toast({
          variant: "destructive",
          title: "Error!",
          description: response.errors[0],
        });
      } else {
        toast({
          title: "Error!",
          description: "There was a problem with your request.",
          action: (
            <ToastAction altText="Try again" onClick={handleLogin}>
              Try again
            </ToastAction>
          ),
        });
      }
    }
    setIsLoading(false);
  };

  const handleResendOTP = async () => {
    if (canResend) {
      setCredentials((prev) => ({ ...prev, otp: "" }));
      await handleLogin();
      resetTimer();
    }
  };

  const getAnnouncement = async () => {
    let response = await getCurrentannouncement("Loginpage");
    if (response.status === "success") {
      setannouncementText(response.data.announcementText);
    }
  };

  const handleVerifyOTP = async () => {
    if (credentials.otp.length !== 4) {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Please Enter correct OTP.",
      });
    } else {
      setIsLoading(true);
      let response = await verifyOTP(credentials);

      if (response.status === "success") {
        toast({
          className: clsx(""),
          description: "OTP verfified successfully.",
        });

        let user = response.data;
        let role = user?.userType?.name.toLowerCase().trim();
        console.log("user", user);
        if (role !== "pensioner" && role !== "community") {
          user["isProfileComplete"] = true;
        }
        updateUser(user);

        resetTimer();

        window.location.href = "/dashboard";
        //}
      } else {
        if (response.status === "conflict") {
          toast({
            variant: "destructive",
            title: "Error!",
            description: response.errors.message,
          });
        } else if (response.status === "unauthorized") {
          toast({
            variant: "destructive",
            title: "Error!",
            description: response.errors,
          });
        } else if (response.status === "notFound") {
          toast({
            variant: "destructive",
            title: "Error!",
            description: response.errors.message,
          });
        } else {
          toast({
            variant: "destructive",
            title: "Error!",
            description: response.errors.message,
            action: (
              <ToastAction altText="Try again" onClick={handleVerifyOTP}>
                Try again
              </ToastAction>
            ),
          });
        }
      }
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen w-full flex flex-col md:flex-row ">
      {/* Left side image - hidden on mobile, visible on md and up */}
      <div className="hidden md:flex md:w-1/2 p-8">
        {/* Logo at top */}
        <div className="absolute top-8 left-8">
          <img src={kmd} alt="KMD logo" className="h-16 w-16" />
        </div>

        {/* Main image */}
        <div className="w-full h-full flex items-center justify-center">
          <img
            src={banner}
            alt="Login banner"
            className="rounded-2xl object-cover w-full h-full shadow-2xl"
          />
        </div>
      </div>

      {/* Right side content */}
      <div className="w-full md:w-1/2 flex flex-col items-center justify-center p-4 md:p-8">
        {/* Mobile logo - only visible on mobile */}
        <div className="md:hidden w-full flex justify-center mb-8">
          <img src={kmd} alt="KMD logo" className="h-16 w-16" />
        </div>

        {/* Announcement Card */}
        {announcementText && (
          <Card className="w-full max-w-md mb-6 bg-white/90 backdrop-blur-sm shadow-xl">
            <CardHeader className="flex flex-row items-center space-x-4 pb-2">
              <Megaphone className="h-6 w-6 text-[#8B0000]" />
              <CardTitle className="text-[#8B0000]">
                Important Announcement
              </CardTitle>
            </CardHeader>
            <CardContent>
              <h2 className="text-lg font-semibold">{announcementText}</h2>
            </CardContent>
          </Card>
        )}

        {/* Login Card */}
        <Card className="w-full max-w-sm bg-white/90 backdrop-blur-sm shadow-xl">
          <CardHeader>
            <CardTitle className="text-2xl text-[#8B0000]">
              Welcome back
            </CardTitle>
            <CardDescription className="text-gray-600">
              Already have an account, Enter your details below.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="text-lg mb-3 font-semibold text-[#8B0000]">
              Login With
            </div>
            <div className="grid gap-4">
              <RadioGroup
                defaultValue="otp"
                value={loginMethod}
                onValueChange={(value) => {
                  setLoginMethod(value);
                  setShowOTP(false);
                  setCredentials((prev) => ({
                    ...prev,
                    otp: "",
                    password: "",
                  }));
                }}
                className="grid grid-cols-2 gap-4 mb-4"
              >
                <div>
                  <RadioGroupItem
                    value="otp"
                    id="otp"
                    className="peer sr-only"
                  />
                  <Label
                    htmlFor="otp"
                    className="group flex flex-col items-center justify-center rounded-lg border-2 border-muted bg-white/50 p-4 hover:border-[#8B0000]/30 peer-data-[state=checked]:border-[#8B0000] peer-data-[state=checked]:bg-[#8B0000] peer-data-[state=checked]:text-white transition-all cursor-pointer shadow-md hover:shadow-lg"
                  >
                    <div className="flex flex-row justify-evenly gap-2 items-center">
                      <Smartphone className="w-6 h-6 text-muted-foreground group-hover:text-[#8B0000]/70 peer-data-[state=checked]:text-white transition-colors" />
                      <span className="font-medium text-sm group-hover:text-[#000000]/70 peer-data-[state=checked]:text-white transition-colors">
                        Mobile
                      </span>
                    </div>
                  </Label>
                </div>
                <div>
                  <RadioGroupItem
                    value="password"
                    id="password"
                    className="peer sr-only"
                  />
                  <Label
                    htmlFor="password"
                    className="group flex flex-col items-center justify-center rounded-lg border-2 border-muted bg-white/50 p-4 hover:border-[#8B0000]/30 peer-data-[state=checked]:border-[#8B0000] peer-data-[state=checked]:bg-[#8B0000] peer-data-[state=checked]:text-white transition-all cursor-pointer shadow-md hover:shadow-lg"
                  >
                    <div className="flex flex-row justify-evenly gap-2 items-center">
                      <KeyRound className="w-6 h-6 text-muted-foreground group-hover:text-[#8B0000]/70 peer-data-[state=checked]:text-white transition-colors" />
                      <span className="font-medium text-sm group-hover:text-[#000000]/70 peer-data-[state=checked]:text-white transition-colors">
                        User Name
                      </span>
                    </div>
                  </Label>
                </div>
              </RadioGroup>

              <div className="grid gap-2">
                <Label htmlFor="mobile" className="text-gray-700">
                  Mobile Number
                </Label>
                <Input
                  id="mobile"
                  type="tel"
                  placeholder="Enter valid number"
                  required
                  disabled={showOTP}
                  value={credentials.phoneNumber}
                  onChange={handleChange("phoneNumber")}
                  maxLength={16}
                  className="bg-white/50"
                />
                {phoneError && (
                  <p className="text-red-500 text-sm">{phoneError}</p>
                )}
              </div>

              {loginMethod === "password" && (
                <div className="grid gap-2">
                  <Label htmlFor="password" className="text-gray-700">
                    Password
                  </Label>
                  <Input
                    id="password"
                    type="password"
                    placeholder="Enter your password"
                    value={credentials.password}
                    onChange={handleChange("password")}
                    className="bg-white/50 border-2 border-gray-200 focus:border-[#8B0000]"
                  />
                </div>
              )}

              {loginMethod === "otp" && !showOTP && (
                <RButton
                  onClick={handleLogin}
                  className="w-full mt-2 bg-[#8B0000] hover:bg-[#660000] text-white transition-colors"
                  isDisabled={
                    isLoading
                    //|| credentials.phoneNumber.length !== 10
                  }
                  isLoading={isLoading}
                >
                  Continue
                </RButton>
              )}

              {loginMethod === "password" && (
                <div>
                  <RButton
                    onClick={handleLogin}
                    className="w-full mt-2 bg-[#8B0000] hover:bg-[#660000] text-white transition-colors"
                    isDisabled={
                      isLoading ||
                      // credentials.phoneNumber.length !== 10 ||
                      !credentials.password
                    }
                    isLoading={isLoading}
                  >
                    Login
                  </RButton>

                  <div className="mt-4 text-center text-sm">
                    <a
                      onClick={() =>
                        navigate(
                          "/resetpassword?phoneNumber=" +
                            credentials.phoneNumber
                        )
                      }
                      className="text-[#8B0000] hover:text-[#660000] underline cursor-pointer"
                    >
                      Reset Password
                    </a>
                  </div>
                </div>
              )}

              {loginMethod === "otp" && showOTP && (
                <div className="grid gap-2">
                  <Label htmlFor="otp" className="text-gray-700">
                    One-Time Password
                  </Label>
                  <InputOTP
                    maxLength={4}
                    pattern="^[0-9]+$"
                    value={credentials.otp}
                    onChange={handleChange("otp")}
                  >
                    <InputOTPGroup>
                      <InputOTPSlot
                        index={0}
                        className="bg-white/50 border-2 border-gray-200 focus:border-[#8B0000]"
                      />
                      <InputOTPSlot
                        index={1}
                        className="bg-white/50 border-2 border-gray-200 focus:border-[#8B0000]"
                      />
                      <InputOTPSlot
                        index={2}
                        className="bg-white/50 border-2 border-gray-200 focus:border-[#8B0000]"
                      />
                      <InputOTPSlot
                        index={3}
                        className="bg-white/50 border-2 border-gray-200 focus:border-[#8B0000]"
                      />
                    </InputOTPGroup>
                  </InputOTP>
                  <CardDescription className="text-red-500">
                    In case you do not get the OTP on your mobile, please
                    contact your operator or admin.
                  </CardDescription>
                  <RButton
                    onClick={handleVerifyOTP}
                    className="w-full bg-[#8B0000] hover:bg-[#660000] text-white transition-colors"
                    isDisabled={isLoading}
                    isLoading={isLoading}
                  >
                    {isLoading ? (
                      <span className="flex items-center">
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Please wait
                      </span>
                    ) : (
                      <span>Login</span>
                    )}
                  </RButton>
                  <RButton
                    onClick={handleResendOTP}
                    variant="outline"
                    className="w-full mt-2 border-[#8B0000] text-[#8B0000] hover:bg-[#8B0000] hover:text-white transition-colors"
                    isDisabled={!canResend}
                  >
                    {canResend ? (
                      <span>Resend OTP</span>
                    ) : (
                      <span>Resend OTP in {countdown}s</span>
                    )}
                  </RButton>
                </div>
              )}
            </div>
            {/* <div className="mt-4 text-center text-sm text-gray-600">
              Don&apos;t have an account?{" "}
              <a
                onClick={() => navigate("/signup")}
                className="text-[#8B0000] hover:text-[#660000] underline cursor-pointer"
              >
                Sign up
              </a>
            </div> */}
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default Login;
