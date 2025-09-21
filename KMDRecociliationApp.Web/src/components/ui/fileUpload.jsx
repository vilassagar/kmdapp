import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { downloadFile } from "@/lib/helperFunctions";
import { cn } from "@/lib/utils";
import { download } from "@/services/files";
import { CloudUploadIcon, X } from "lucide-react";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Badge } from "./badge";
import { toast } from "./use-toast";

export default function FileUpload({
  onChange,
  value,
  error,
  accept,
  isImage,
}) {
  const [doc, setDoc] = useState({
    id: 0,
    name: "",
    file: null,
    url: "",
  });

  useEffect(() => {
    (async () => {
      if (value !== null) {
        if (isImage && value?.url?.length) {
          let response = await download(value.id, value.name, value.url);
          if (response.status === "success") {
            let url = window.URL.createObjectURL(new Blob([response.data]));
            value.fileData = url;
          } else {
            toast({
              variant: "destructive",
              title: "Error.",
              description: "Unable to get QR Code photo.",
            });
          }
        }

        setDoc(value);
      }
    })();
  }, [value]);

  const handleChange = (event) => {
    let newDoc = { ...doc };
    if (event?.target?.files?.length) {
      // set file name
      // set file
      // remove url
      let file = event.target.files[0];
      newDoc["name"] = file.name;
      newDoc["file"] = file;
      newDoc["url"] = "";
    } else {
      // remove file, name , url
      newDoc = null;
    }

    setDoc(newDoc);
    onChange(newDoc);
  };

  const handleBadgeClick = async (event) => {
    let url = "";
    if (doc?.url?.length) {
      //get file and download
      let response = await download(
        doc?.id || 0,
        doc?.name || "",
        doc?.url || ""
      );

      if (response.status === "success") {
        url = URL.createObjectURL(new Blob([response.data]));
        downloadFile(url, doc?.name);
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to download file.",
        });
      }
    }

    if (doc?.file) {
      url = URL.createObjectURL(new Blob([doc?.file]));
      downloadFile(url, doc?.name);
    }
  };

  return (
    <Card
      className={cn(
        error?.length && `border-red-600`,
        "border-dashed shadow-none"
      )}
    >
      <CardContent className="p-2 ">
        {doc?.file || doc?.url ? (
          <div className="flex flex column justify-between items-start ">
            {isImage ? (
              <img
                src={
                  doc?.url?.length
                    ? doc?.fileData
                    : URL.createObjectURL(doc?.file)
                }
                alt="Cheque Preview"
                style={{ maxWidth: "300px", marginTop: "10px" }}
              />
            ) : (
              <Badge onClick={handleBadgeClick} className="cursor-pointer">
                {doc?.name}
              </Badge>
            )}
            <Button
              variant="ghost"
              onClick={handleChange}
              size="small"
              className="px-1"
            >
              <X className="h-4 w-4" />
            </Button>
          </div>
        ) : (
          <div className=" border-red-500">
            <div className="group relative ">
              <Button variant="outline" size="sm">
                <CloudUploadIcon className="h-4 w-4 mr-2" />
                Upload File
              </Button>
              <input
                type="file"
                className="absolute inset-0 h-full w-full cursor-pointer opacity-0"
                onChange={handleChange}
                accept={accept}
              />
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
}

FileUpload.defaultProps = {
  onChange: () => {},
  value: null,
  error: "",
  accept: "",
  isImage: false,
};

FileUpload.propTypes = {
  onChange: PropTypes.func,
  value: PropTypes.object,
  error: PropTypes.string,
  accept: PropTypes.string,
  isImage: PropTypes.bool,
};
