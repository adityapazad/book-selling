document.querySelector(".homeBtn").addEventListener("click", () => {
  window.location.href = "index.html";
});

document.getElementById('submit').addEventListener('click', () => {
  let input = document.getElementById('input')
  let text = input.value.trim()

  movieApi(text)
  input.value = ""
  console.log(text);
})


async function movieApi(movieName) {
  try {
    let response = await fetch(
      `http://www.omdbapi.com/?s=${movieName}&apikey=de7526dc`);
    let data = await response.json()

    console.log(data);

    let output = document.querySelector(".movies");

    if(data.Response == "True"){
      output.innerHTML = ''; // Clear previous results
      data.Search.forEach(movie => {
        output.innerHTML += `
          <div class="movie-card">
            <img src="${movie.Poster}" alt="${movie.Title}">
            <div class="movie-details">
              <h5>${movie.Title}</h5>
              <p>${movie.Year}</p>
            </div>
          </div>
        `;
      });
    } else {
      output.innerHTML = `
        <p>No result found for "${movieName}"</p>
      `
    }
    

  } catch (error) {
    console.log(error);
  }
}