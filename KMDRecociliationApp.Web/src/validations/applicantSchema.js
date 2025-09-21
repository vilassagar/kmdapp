import * as yup from "yup";

const applicantSchema = () => {
  return yup.object().shape({
    firstName: yup
      .string()
      .required("First Name is required")
      .max(100, "First Name cannot exceed 100 characters"),
    
    lastName: yup
      .string()
      .nullable(),
    
 
    
    dateOfBirth: yup
      .date()
      .required("Date of Birth is required")
      .typeError("Invalid date format"),
    
    email: yup.string().email("Invalid Email"),
   
    gender: yup
    .mixed() // Use mixed instead of object to allow both
    .required("Gender is required")
    .test('isValidGender', 'Gender is required', function(value) {
      return value !== null && (typeof value === 'object' ? !!value.id : !!value);
    }),
        
    mobileNumber: yup
      .string()
      .required("Contact Number is required")
      .matches(/^[0-9]*$/, "Contact Number must contain only digits")
      .max(15, "Contact Number cannot exceed 15 characters")
      .nullable(),
    
    salary: yup
      .number()
      .required("Salary is required")
      .min(0, "Salary must be a positive value")
      .typeError("Salary must be a number"),
  
    associatedOrganization: yup
      .string()
      .required("Associated Organization is required")
      .max(200, "Associated Organization cannot exceed 200 characters"),
    productName: yup
      .string()
      .required("Product Name is required")
      .max(200, "Product Name cannot exceed 200 characters"),
      
    
    address: yup
      .string()
      .required("Location/Address is required")
      .max(200, "Location/Address cannot exceed 200 characters"),
    
      idCardType: yup
      .mixed() // Use mixed instead of object to allow both
      .required("ID Card Type is required")
      .test('isValidIdCardType', 'ID Card Type is required', function(value) {
        return value !== null && (typeof value === 'object' ? !!value.id : !!value);
      }),
    
    
    idCardNumber:yup.string().required("ID Card Number is required")
      .max(50, "ID Card Number cannot exceed 50 characters"),
    
    bankDetails: yup.object().shape({
      bankName: yup.string().nullable(),
      bankBranchDetails: yup.string().nullable(),      
      bankAccountNumber: yup.string().nullable(),
      bankIfscCode: yup.string().nullable(),
      bankMicrCode: yup.string().nullable(),
    }).nullable(),
    
    dependents: yup
      .array()
      .of(
        yup.object().shape({
          firstName: yup.string().nullable(),
          lastName: yup.string().nullable(),
          relationship: yup.string().nullable(),
          dateOfBirth: yup.date().nullable().typeError("Invalid date format"),
          contactNumber: yup.string().nullable(),
        })
      )
      .nullable(),
  });
};

export default applicantSchema;