import { WrenchIcon } from "lucide-react";

export default function Maintenance() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gradient-to-b from-gray-100 to-gray-200 p-4">
      <div className="max-w-md w-full space-y-8 text-center">
        <WrenchIcon className="mx-auto h-16 w-16 text-gray-600 animate-bounce" />
        <h2 className="mt-6 text-3xl font-extrabold text-gray-900">
          We&apos;re currently performing some maintenance on our site.
          We&apos;ll be back up shortly. Thanks for your patience!
        </h2>
        <p className="mt-2 text-sm text-gray-600">We&apos;ll be back soon!</p>
      </div>
    </div>
  );
}
