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
import { Textarea } from "./textarea";
import { Label } from "./label";

const Messages = ({ messages, updateMessages }) => {
  const [messageList, setMassageList] = useState([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newMessage, setNewMessage] = useState({
    id: 0,
    name: "",
    template: "",
    status: false,
  });
  const [isEditing, setIsEditing] = useState(false);
  const [editIndex, setEditIndex] = useState(null);
  const [errors, setErrors] = useState({});

  useEffect(() => {
    setMassageList(messages);
  }, [messages]);

  const handleMessageRemove = (index) => {
    const newList = produce(messageList, (draft) => {
      draft.splice(index, 1);
    });
    setMassageList(newList);
    updateMessages(newList);
  };

  const handleMessageEdit = (index) => {
    setNewMessage(messageList[index]);
    setEditIndex(index);
    setIsEditing(true);
    setIsModalOpen(true);
  };

  const validate = async () => {
    try {
      await Yup.object({
        id: Yup.number().required("Id is required"),
        name: Yup.string()
          .required("Name is required")
          .min(2, "Name must be at least 2 characters"),
        template: Yup.string()
          .required("Template is required")
          .min(2, "Template must be at least 2 characters"),
        status: Yup.boolean().nullable(),
      }).validate(newMessage, { abortEarly: false });

      return true;
    } catch (error) {
      const errors = {};
      error.inner.forEach((e) => {
        errors[e.path] = e.message;
      });
      setErrors(errors);
      return false;
    }
  };

  const handleAddOrUpdateMessage = async () => {
    const isValid = await validate();
    if (isValid) {
      const newList = produce(messageList, (draft) => {
        if (isEditing && editIndex !== null) {
          draft[editIndex] = newMessage;
        } else {
          draft.push(newMessage);
        }
      });
      setMassageList(newList);
      updateMessages(newList);
      setNewMessage({
        name: "",
        template: "",
        status: false,
      });
      setErrors({});
      setIsEditing(false);
      setEditIndex(null);
      setIsModalOpen(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setNewMessage({ ...newMessage, [name]: value });
  };

  return (
    <div className="">
      <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
        <div>
          <h4 className="text-sm font-medium leading-none">Message Details</h4>
          <Separator className="my-2" />
        </div>

        <div className="rounded-lg shadow-lg ">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead> Name</TableHead>
                <TableHead> Template</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">
                  <DialogTrigger>
                    <Button
                      variant="outline"
                      onClick={() => {
                        setIsEditing(false);
                        setNewMessage({
                          id: 0,
                          name: "",
                          template: "",
                          status: false,
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
              {messageList?.length ? (
                messageList.map((message, index) => (
                  <TableRow key={index}>
                    <TableCell>{message.name}</TableCell>
                    <TableCell>{message.template}</TableCell>
                    <TableCell>
                      {message.status ? "Active" : "Inactive"}
                    </TableCell>
                    <TableCell className="text-right">
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => handleMessageEdit(index)}
                      >
                        <FilePenIcon className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => handleMessageRemove(index)}
                      >
                        <Trash2Icon className="h-4 w-4" />
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={7} className="h-24 text-center">
                    No messages found.
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
                label="Name"
                type="text"
                name="name"
                value={newMessage.name}
                onChange={handleChange}
                error={errors?.name}
              />
              {errors.name && <div className="text-red-500">{errors.name}</div>}
            </div>
            <div className="mt-5">
              <Label className="mb-3">Template</Label>
              <Textarea
                type="text"
                name="template"
                value={newMessage.template}
                onChange={handleChange}
                error={errors?.template}
              />
              {errors.template && (
                <div className="text-red-500">{errors.template}</div>
              )}
            </div>
          </div>
          <Button onClick={handleAddOrUpdateMessage}>
            {isEditing ? "Update Message" : "Save Message"}
          </Button>
        </DialogContent>
      </Dialog>
    </div>
  );
};

Messages.defaultProps = {
  messages: [],
  updateMessages: () => {},
};

Messages.propTypes = {
  messages: PropTypes.array,
  updateMessages: PropTypes.func,
};

export default Messages;
