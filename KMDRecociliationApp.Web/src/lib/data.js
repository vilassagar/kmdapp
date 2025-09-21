export const months = [
  { id: 1, name: "January" },
  { id: 2, name: "February" },
  { id: 3, name: "March" },
  { id: 4, name: "April" },
  { id: 5, name: "May" },
  { id: 6, name: "June" },
  { id: 7, name: "July" },
  { id: 8, name: "August" },
  { id: 9, name: "September" },
  { id: 10, name: "October" },
  { id: 11, name: "November" },
  { id: 12, name: "December" },
];

export const getYears = () => {
  // Get the current year
  const currentYear = new Date().getFullYear();

  // Create an array of years from 1900 to the current year
  const years = [];

  for (let year = 1900; year <= currentYear; year++) {
    years.push({ id: year, name: year.toString() });
  }

  return years;
};
