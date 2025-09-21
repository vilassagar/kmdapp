// import * as yup from "yup";

// const userProfileSchema = () => {
//   return yup.object().shape({
//     roles: yup.array().when("$userType", {
//       is: (userType) =>
//         userType !== null && (userType.toLowerCase().trim() !== "pensioner" 
//       || userType.toLowerCase().trim() !== "community" ),
//       then: () =>
//         yup
//           .array()
//           .of(
//             yup.object().shape({
//               id: yup.number(),
//               name: yup.string(),
//             })
//           )
//           .min(1, "Role is required"),
//       otherwise: () => yup.array(),
//     }),
//     firstName: yup.string().required("First Name is required"),
//     lastName: yup.string().required("Last Name is required"),
//     email: yup.string().email("Invalid Email").notRequired(),
//     password: yup.string().notRequired(),
    
//     empId: yup.string().when("$userType", {
//       is: (userType) =>
//         userType !== null && userType.toLowerCase().trim() === "pensioner",
//       then: () => yup.string().required("Emp ID is required"),
//       otherwise: () => yup.string().notRequired(),
//     }),
//     mobileNumber: yup.string().required("Mobile Number is required"),
//     organisation: yup.string().when("$userType", {
//       is: (userType) =>
//         userType !== null && userType.toLowerCase().trim() === "association",
//       then: () => yup.object().required("Organisation is required"),
//       otherwise: () => yup.object().notRequired().nullable(),
//     }),
//     association: yup.object().when("$userType", {
//       is: (userType) =>
//         (userType !== null && userType.toLowerCase().trim() === "pensioner") ||
//         (userType !== null && userType.toLowerCase().trim() === "association"),
//       then: () => yup.object().required("Association is required"),
//       otherwise: () => yup.object().notRequired().nullable(),
//     }),
//     dateOfBirth: yup.string().required("Date Of Birth is required"),
//     //gender: yup.object().nullable(),
//     gender: yup.object().when("$userType", {
//       is: (userType) =>
//         (userType !== null && userType.toLowerCase().trim() === "pensioner") ||
//         (userType !== null && userType.toLowerCase().trim() === "association"),
//       then: () => yup.object().required("Gender is required"),
//       otherwise: () => yup.object().notRequired().nullable(),
//     }),
//     pensioneridtype: yup.object().nullable(),
//     state: yup.object().when("$userType", {
//       is: (userType) =>
//         userType !== null && userType.toLowerCase().trim() === "pensioner",
//       then: () => yup.object().required("Sate is required"),
//       otherwise: () => yup.object().notRequired().nullable(),
//     }),
//     pincode: yup.string().when("$userType", {
//       is: (userType) =>
//         userType !== null && userType.toLowerCase().trim() === "pensioner",
//       then: () => yup.string().required("Pin code is required"),
//       otherwise: () => yup.string().notRequired(),
//     }),
//     // uniqueidnumber: yup.string().when("$userType", {
//     //   is: (userType) =>
//     //     userType !== null && userType.toLowerCase().trim() === "pensioner",
//     //   then: () => yup.string().required("Unique Id Number is required"),
//     //   otherwise: () => yup.string().notRequired(),
//     // }),
//     uniqueidnumber: yup.string().when(["$userType", "pensioneridtype"], {
//       is: (userType, pensioneridtype) =>
//         userType !== null && 
//         userType.toLowerCase().trim() === "pensioner" &&
//         pensioneridtype?.id === 2,
//       then: () => 
//         yup.string()
//           // .required("PAN Card Number is required")
//           .matches(
//             /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/, 
//             "Invalid PAN Card format. It should be in format: ABCDE1234F"
//           ),
//       otherwise: () => 
//         yup.string().when("$userType", {
//           is: (userType) =>
//             userType !== null && userType.toLowerCase().trim() === "pensioner",
//           then: () => 
//             yup.string()
//               .required("Unique Id Number is required")
//               .matches(/^\d{10}$/, "Unique Id Number must be exactly 10 digits")
//               .test(
//                 "is-number",
//                 "Unique Id Number must contain only numbers",
//                 value => !isNaN(value)
//               ),
//           otherwise: () => yup.string().notRequired(),
//         }),
//     }),
//     address: yup.string().when("$userType", {
//       is: (userType) =>
//         userType !== null && userType.toLowerCase().trim() === "pensioner",
//       then: () => yup.string().required("Address is required"),
//       otherwise: () => yup.string().notRequired(),
//     }),
//   });
// };

// export default userProfileSchema;
import * as yup from "yup";

const userProfileSchema = () => {
  return yup.object().shape({
    roles: yup.array().when("$userType", {
      is: (userType) =>
        userType !== null && 
        (userType.toLowerCase().trim() === "internaluser" || 
         userType.toLowerCase().trim() === "association"),
      then: () =>
        yup
          .array()
          .of(
            yup.object().shape({
              id: yup.number(),
              name: yup.string(),
            })
          )
          .min(1, "Role is required"),
      otherwise: () => yup.array(),
    }),
    firstName: yup.string().required("First Name is required"),
    lastName: yup.string().required("Last Name is required"),
    email: yup.string().email("Invalid Email").notRequired(),
    password: yup.string().notRequired(),
    
    empId: yup.string().when("$userType", {
      is: (userType) =>
        userType !== null && userType.toLowerCase().trim() === "pensioner",
      then: () => yup.string().required("Emp ID is required"),
      otherwise: () => yup.string().notRequired(),
    }),
    mobileNumber: yup.string().required("Mobile Number is required"),
    organisation: yup.object().when("$userType", {
      is: (userType) =>
        userType !== null && 
        (userType.toLowerCase().trim() === "association" || 
         userType.toLowerCase().trim() === "community"),
      then: () => yup.object().required("Organisation is required"),
      otherwise: () => yup.object().notRequired().nullable(),
    }),
    association: yup.object().when("$userType", {
      is: (userType) =>
        userType !== null && 
        (userType.toLowerCase().trim() === "pensioner" || 
         userType.toLowerCase().trim() === "association" ||
         userType.toLowerCase().trim() === "community"),
      then: () => yup.object().required("Association is required"),
      otherwise: () => yup.object().notRequired().nullable(),
    }),
    dateOfBirth: yup.string().required("Date Of Birth is required"),
    gender: yup.object().when("$userType", {
      is: (userType) =>
        userType !== null && 
        (userType.toLowerCase().trim() === "pensioner" || 
         userType.toLowerCase().trim() === "association" ||
         userType.toLowerCase().trim() === "community"),
      then: () => yup.object().required("Gender is required"),
      otherwise: () => yup.object().notRequired().nullable(),
    }),
    pensioneridtype: yup.object().nullable(),
    state: yup.object().when("$userType", {
      is: (userType) =>
        userType !== null && 
        (userType.toLowerCase().trim() === "pensioner" ||
         userType.toLowerCase().trim() === "community"),
      then: () => yup.object().required("State is required"),
      otherwise: () => yup.object().notRequired().nullable(),
    }),
    pincode: yup.string().when("$userType", {
      is: (userType) =>
        userType !== null && 
        (userType.toLowerCase().trim() === "pensioner" ||
         userType.toLowerCase().trim() === "community"),
      then: () => yup.string().required("Pin code is required"),
      otherwise: () => yup.string().notRequired(),
    }),
    uniqueidnumber: yup.string().when(["$userType", "pensioneridtype"], {
      is: (userType, pensioneridtype) =>
        userType !== null && 
        userType.toLowerCase().trim() === "pensioner" &&
        pensioneridtype?.id === 2,
      then: () => 
        yup.string()
          .matches(
            /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/, 
            "Invalid PAN Card format. It should be in format: ABCDE1234F"
          ),
      otherwise: () => 
        yup.string().when("$userType", {
          is: (userType) =>
            userType !== null && userType.toLowerCase().trim() === "pensioner",
          then: () => 
            yup.string()
              .required("Unique Id Number is required")
              .matches(/^\d{10}$/, "Unique Id Number must be exactly 10 digits")
              .test(
                "is-number",
                "Unique Id Number must contain only numbers",
                value => !isNaN(value)
              ),
          otherwise: () => yup.string().notRequired(),
        }),
    }),
    address: yup.string().when("$userType", {
      is: (userType) =>
        userType !== null && 
        (userType.toLowerCase().trim() === "pensioner" ||
         userType.toLowerCase().trim() === "community"),
      then: () => yup.string().required("Address is required"),
      otherwise: () => yup.string().notRequired(),
    }),
  });
};

export default userProfileSchema;