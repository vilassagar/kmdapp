/* eslint-disable no-undef */
/* eslint-disable react/prop-types */
import React, { useState, useEffect } from "react";
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
import RButton from "@/components/ui/rButton";
import { useToast } from "@/components/ui/use-toast";
import {  resetPasswordOTP,resetPasswordverifyOTP,updateresetPassword } from "@/services/authenticate";
import { useNavigate } from "react-router-dom";

const ResetPassword = ({ onClose }) => {
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const phoneNumber = params.get("phoneNumber");
  const navigate = useNavigate()
  const { toast } = useToast();
  const [step, setStep] = useState(1); 
  const [isLoading, setIsLoading] = useState(false);
  const [countdown, setCountdown] = useState(30);
  const [canResend, setCanResend] = useState(false);
  const [formData, setFormData] = useState({
    phoneNumber: phoneNumber,
    otp: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [errors, setErrors] = useState({
    phoneNumber: "",
    newPassword: "",
    confirmPassword: "",
  });
  useEffect(() => {
    let timer;
    if (step === 2 && countdown > 0) {
      timer = setTimeout(() => setCountdown(countdown - 1), 1000);
    } else if (countdown === 0) {
      setCanResend(true);
    }
    return () => clearTimeout(timer);
  }, [step, countdown]);

  const validatePhone = (phone) => {
    if (phone.length !== 10) {
      return "Please enter a valid 10-digit mobile number";
    }
    return "";
  };

  const validatePassword = () => {
    const { newPassword, confirmPassword } = formData;
    const errors = {
      newPassword: "",
      confirmPassword: "",
    };

    if (newPassword.length < 8) {
      errors.newPassword = "Password must be at least 8 characters long";
    }

    if (newPassword !== confirmPassword) {
      errors.confirmPassword = "Passwords do not match";
    }

    return errors;
  };

  const handleChange = (name) => (event) => {
    let value = name === "otp" ? event : event.target.value;

    if (name === "phoneNumber") {
      value = value.replace(/\D/g, "").slice(0, 10);
      const phoneError = validatePhone(value);
      setErrors((prev) => ({ ...prev, phoneNumber: phoneError }));
    }

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const resetTimer = () => {
    setCountdown(30);
    setCanResend(false);
  };

  const handleRequestOTP = async () => {
    const phoneError = validatePhone(formData.phoneNumber);
    if (phoneError) {
      setErrors((prev) => ({ ...prev, phoneNumber: phoneError }));
      return;
    }

    setIsLoading(true);
    try {
      const response = await resetPasswordOTP({phoneNumber: formData.phoneNumber});

      if (response.status === "success") {
        toast({
          description: "OTP sent successfully.",
        });
        setStep(2);
        resetTimer();
      } else {
        toast({
          variant: "destructive",
          title: "Error!",
          description: response.errors?.[0] || "Failed to send OTP",
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

  const handleVerifyOTP = async () => {
    if (formData.otp.length !== 4) {
      toast({
        variant: "destructive",
        description: "Please enter a valid OTP",
      });
      return;
    }

    setIsLoading(true);
    try {
      const response = await resetPasswordverifyOTP({
        phoneNumber: formData.phoneNumber,
        otp: formData.otp,
      });

      if (response.status === "success") {
        toast({
          description: "OTP verified successfully",
        });
        setStep(3);
      } else {
        toast({
          variant: "destructive",
          title: "Error!",
          description: response.errors?.[0] || "Invalid OTP",
        });
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Error!",
        description: "There was a problem verifying the OTP",
      });
    }
    setIsLoading(false);
  };

  const handleResetPassword = async () => {
    const passwordErrors = validatePassword();
    setErrors((prev) => ({ ...prev, ...passwordErrors }));

    if (passwordErrors.newPassword || passwordErrors.confirmPassword) {
      return;
    }

    setIsLoading(true);
    try {
      const response = await updateresetPassword({
        phoneNumber: formData.phoneNumber,
       password: formData.newPassword,
        confirmPassword: formData.confirmPassword,
      });

      if (response.status === "success") {
        toast({
          description: "Password reset successfully",
        });
        navigate('/login')
        // onClose();
      } else {
        toast({
          variant: "destructive",
          title: "Error!",
          description: response.errors?.[0] || "Failed to reset password",
        });
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Error!",
        description: "There was a problem resetting your password",
      });
    }
    setIsLoading(false);
  };

  const renderStep = () => {
    switch (step) {
      case 1:
        return (
          <>
            <div className="grid gap-2">
              <Label htmlFor="mobile">Mobile Number</Label>
              <Input
                id="mobile"
                type="tel"
                placeholder="Enter 10-digit number"
                value={formData.phoneNumber}
                onChange={handleChange("phoneNumber")}
                maxLength={10}
                className={errors.phoneNumber ? "border-red-500" : ""}
              />
              {errors.phoneNumber && (
                <p className="text-red-500 text-sm">{errors.phoneNumber}</p>
              )}
            </div>
            <RButton
              onClick={handleRequestOTP}
              className="w-full mt-4"
              isDisabled={isLoading || formData.phoneNumber.length !== 10}
              isLoading={isLoading}
            >
              Send OTP
            </RButton>
          </>
        );

      case 2:
        return (
          <div className="grid gap-2">
            <Label htmlFor="otp">One-Time Password</Label>
            <InputOTP
              maxLength={4}
              value={formData.otp}
              onChange={handleChange("otp")}
            >
              <InputOTPGroup>
                <InputOTPSlot index={0} />
                <InputOTPSlot index={1} />
                <InputOTPSlot index={2} />
                <InputOTPSlot index={3} />
              </InputOTPGroup>
            </InputOTP>
            <RButton
              onClick={handleVerifyOTP}
              className="w-full mt-4"
              isDisabled={isLoading || formData.otp.length !== 4}
              isLoading={isLoading}
            >
              Verify OTP
            </RButton>
            <RButton
              onClick={handleRequestOTP}
              variant="outline"
              className="w-full mt-2"
              isDisabled={!canResend || isLoading}
            >
              {canResend ? (
                <span>Resend OTP</span>
              ) : (
                <span>Resend OTP in {countdown}s</span>
              )}
            </RButton>
          </div>
        );

      case 3:
        return (
          <div className="grid gap-2">
            <div className="grid gap-2">
              <Label htmlFor="newPassword">New Password</Label>
              <Input
                id="newPassword"
                type="password"
                placeholder="Enter new password"
                value={formData.newPassword}
                onChange={handleChange("newPassword")}
              />
              {errors.newPassword && (
                <p className="text-red-500 text-sm">{errors.newPassword}</p>
              )}
            </div>
            <div className="grid gap-2">
              <Label htmlFor="confirmPassword">Confirm Password</Label>
              <Input
                id="confirmPassword"
                type="password"
                placeholder="Confirm new password"
                value={formData.confirmPassword}
                onChange={handleChange("confirmPassword")}
              />
              {errors.confirmPassword && (
                <p className="text-red-500 text-sm">{errors.confirmPassword}</p>
              )}
            </div>
            <RButton
              onClick={handleResetPassword}
              className="w-full mt-4"
              isDisabled={isLoading}
              isLoading={isLoading}
            >
              Reset Password
            </RButton>
          </div>
        );

      default:
        return null;
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen">
      <Card className="mx-auto max-w-sm">
        <CardHeader>
          <CardTitle>Reset Password</CardTitle>
          <CardDescription>
            {step === 1 && "Enter your mobile number to reset your password"}
            {step === 2 && "Enter the OTP sent to your mobile number"}
            {step === 3 && "Enter your new password"}
          </CardDescription>
        </CardHeader>
        <CardContent>{renderStep()}</CardContent>
      </Card>
    </div>
  );
};

export default ResetPassword;
