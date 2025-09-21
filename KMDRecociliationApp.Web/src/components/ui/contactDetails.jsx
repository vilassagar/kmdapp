import React, { useState, useEffect } from "react";
import { produce } from "immer";
import * as Yup from "yup";
import { Separator } from "./separator";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "./table";
import { Button } from "./button";
import RInput from "./rInput";
import { Trash2Icon, FilePenIcon } from "lucide-react";
import { Dialog, DialogContent } from "./dialog";
import { DialogTrigger } from "@radix-ui/react-dialog";
import PropTypes from "prop-types";

const ContactDetails = ({ contacts, updateContacts }) => {
  const [contactList, setContactList] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newContact, setNewContact] = useState({
    id: 0,
    firstName: "",
    lastName: "",
    phone: "",
    email: "",
  });
  const [isEditing, setIsEditing] = useState(false);
  const [editIndex, setEditIndex] = useState(null);
  const [errors, setErrors] = useState({});

  useEffect(() => {
    setContactList(contacts);
  }, [contacts]);

  const handleContactRemove = (index) => {
    const newList = produce(contactList, (draft) => {
      draft.splice(index, 1);
    });
    setContactList(newList);
    updateContacts(newList);
  };

  const handleContactEdit = (index) => {
    setNewContact(contactList[index]);
    setEditIndex(index);
    setIsEditing(true);
    setIsModalOpen(true);
  };

  const validate = () => {
    try {
      Yup.object({
        id: Yup.number().required("Id is required"),
        firstName: Yup.string()
          .required("First Name is required")
          .min(2, "First Name must be at least 2 characters"),
        lastName: Yup.string()
          .required("Last Name is required")
          .min(2, "Last Name must be at least 2 characters"),
        phone: Yup.string()
          .required("Phone Number is required")
          .matches(/^[0-9]+$/, "Phone Number must be digits only")
          .min(10, "Phone Number must be at least 10 digits")
          .max(15, "Phone Number must be at most 15 digits"),
        email: Yup.string()
          .required("Email is required")
          .email("Invalid email address"),
      }).validateSync(newContact, { abortEarly: false });
      return true;
    } catch (error) {
      const errorMessages = error.inner.reduce((acc, curr) => {
        acc[curr.path] = curr.message;
        return acc;
      }, {});
      setErrors(errorMessages);
      return false;
    }
  };

  const handleAddOrUpdateContact = () => {
    const isValid = validate();
    if (isValid) {
      const newList = produce(contactList, (draft) => {
        if (isEditing && editIndex !== null) {
          draft[editIndex] = newContact;
        } else {
          draft.push(newContact);
        }
      });
      setContactList(newList);
      updateContacts(newList);
      setNewContact({
        id: 0,
        firstName: "",
        lastName: "",
        phone: "",
        email: "",
      });
      setErrors({});
      setIsEditing(false);
      setEditIndex(null);
      setIsModalOpen(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setNewContact({ ...newContact, [name]: value });
  };

  return (
    <div>
      <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
        <div>
          <h4 className="text-sm font-medium leading-none">Contact Details</h4>
          <Separator className="my-2" />
        </div>

        <div className="rounded-lg shadow-lg">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>First Name</TableHead>
                <TableHead>Last Name</TableHead>
                <TableHead>Phone</TableHead>
                <TableHead>Email</TableHead>
                <TableHead className="text-right">
                  <DialogTrigger>
                    <Button
                      variant="outline"
                      onClick={() => {
                        setIsEditing(false);
                        setNewContact({
                          id: 0,
                          firstName: "",
                          lastName: "",
                          phone: "",
                          email: "",
                        });
                      }}
                    >
                      +
                    </Button>
                  </DialogTrigger>
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {contactList?.length ? (
                contactList.map((contact, index) => (
                  <TableRow key={index}>
                    <TableCell>{contact.firstName}</TableCell>
                    <TableCell>{contact.lastName}</TableCell>
                    <TableCell>{contact.phone}</TableCell>
                    <TableCell>{contact.email}</TableCell>
                    <TableCell className="text-right">
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => handleContactEdit(index)}
                      >
                        <FilePenIcon className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => handleContactRemove(index)}
                      >
                        <Trash2Icon className="h-4 w-4" />
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={7} className="h-24 text-center">
                    No contacts found.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
        <DialogContent>
          <div>
            <div>
              <RInput
                label="First Name"
                isRequired={true}
                type="text"
                name="firstName"
                value={newContact.firstName}
                onChange={handleChange}
                error={errors?.firstName}
              />
              {errors.firstName && (
                <div className="text-red-500">{errors.firstName}</div>
              )}
            </div>
            <div>
              <RInput
                label="Last Name"
                isRequired={true}
                type="text"
                name="lastName"
                value={newContact.lastName}
                onChange={handleChange}
                error={errors?.lastName}
              />
              {errors.lastName && (
                <div className="text-red-500">{errors.lastName}</div>
              )}
            </div>
            <div>
              <RInput
                label="Phone Number"
                isRequired={true}
                type="text"
                name="phone"
                value={newContact.phone}
                onChange={handleChange}
                error={errors?.phone}
              />
              {errors.phone && (
                <div className="text-red-500">{errors.phone}</div>
              )}
            </div>
            <div>
              <RInput
                label="Email"
                isRequired={true}
                type="text"
                name="email"
                value={newContact.email}
                onChange={handleChange}
                error={errors?.email}
              />
              {errors.email && (
                <div className="text-red-500">{errors.email}</div>
              )}
            </div>
          </div>
          <Button onClick={handleAddOrUpdateContact}>
            {isEditing ? "Update Contact" : "Save Contact"}
          </Button>
        </DialogContent>
      </Dialog>
    </div>
  );
};

ContactDetails.defaultProps = {
  contacts: [],
  updateContacts: () => {},
};

ContactDetails.propTypes = {
  contacts: PropTypes.array,
  updateContacts: PropTypes.func,
};

export default ContactDetails;
