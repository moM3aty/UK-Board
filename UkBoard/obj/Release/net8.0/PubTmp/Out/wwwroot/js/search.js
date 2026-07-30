 document.addEventListener('DOMContentLoaded', function () {
  'use strict';

const certificates = {
  "reem mohamed": "../images//team1.jpg",
  "12345": "../images//team2.jpg",
  "ahmed said": "../images//team3.jpg"
};

const input = document.getElementById("searchInput");
const display = document.getElementById("certificateDisplay");

input.addEventListener("input", () => {
  const value = input.value.trim().toLowerCase();

  if (value === "") {
    display.innerHTML = "";
    return;
  }

  if (certificates[value]) {
    display.innerHTML = `
      <img src="${certificates[value]}" alt="Certificate for ${value}" class="img-fluid rounded shadow">
    `;
  } else {
    display.innerHTML = `
      <p class="text-danger text-center mt-2">No certificate found for "${input.value}"</p>
    `;
  }
});
 })