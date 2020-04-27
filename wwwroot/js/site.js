// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(() => {
    $('#searchForm').on('submit', (e) => {
        let searchText = $('#searchText').val();
        getMovies(searchText);
        e.preventDefault();
    });
});
function getMovies(searchText){
    axios.get('http://www.omdbapi.com/?s='+searchText+'&apikey=37f20733')
    .then((response) => {
        let movies = response.data.Search;
        let output = '';
        $.each(movies, (index, movie)=> {
            output += `
            
            <div class="card-group col-md-3 mb-3 p-2">
                <div class="card text-white bg-dark" style="width: 20rem;">
                    <img src="${movie.Poster}" class="card-img-top" alt="No image available">
                    <div class="card-body">
                        <h6 class="card-title">${movie.Title}</h6>
                        <a onclick="movieSelected('${movie.imdbID}')" class="btn-sm btn-info" href='#'>Movie Details</a>
                    </div>
                </div>
            </div>
            `;
        });

        $('#movies').html(output);
    })
    .catch((err)=>{
        console.log(err);
    });
}

function movieSelected(id){
    sessionStorage.setItem('movieId', id);
    window.location = 'show';
    return false;
}
function getMovie(){
    let movieId=sessionStorage.getItem('movieId');
    axios.get('http://www.omdbapi.com/?i='+movieId+'&apikey=37f20733')
    .then((response) => {
        
        let movie=response.data;
        let output =`
            <div class="row">
                <div class="col-md-4">
                    <img src="${movie.Poster}">
                </div>
                <div class="col-md-8">
                    <h2 style="color:white">${movie.Title}</h2>
                    <ul class="list-group">
                        <li class="list-group-item"><strong>Genre: </strong>${movie.Genre}</li>
                        <li class="list-group-item"><strong>Released: </strong>${movie.Released}</li>
                        <li class="list-group-item"><strong>Rated: </strong>${movie.Rated}</li>
                        <li class="list-group-item"><strong>IMDB Rating: </strong>${movie.imdbRating}</li>
                        <li class="list-group-item"><strong>Director: </strong>${movie.Director}</li>
                        <li class="list-group-item"><strong>Writer: </strong>${movie.Writer}</li>
                        <li class="list-group-item"><strong>Actors: </strong>${movie.Actors}</li>

                    </ul> 
                </div>
            </div>
            <div class="row">
                <div class="well m-5" style="color:white">
                    <h3>Plot</h3>
                    ${movie.Plot}
                    <hr>
                    <a href="http://imdb.com/title/${movie.imdbID}" target="_blank" class="btn btn-info">View IMDB</a>
                </div>
            </div>
        `;
        $('#movie').html(output);
    })
    .catch((err)=>{
        console.log(err);
    });
}

getMovie();
