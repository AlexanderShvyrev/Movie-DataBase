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
            
            <div class="card border-success col-md-2 card-group col-md-2 m-2 p-2" >
                <div class="card text-primary bg-dark" style="width: 20rem;">
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
            <div class="row text-primary">
                <div class="col-md-4">
                    <img src="${movie.Poster}">
                </div>
                <div class="col-md-8">
                    <h2>${movie.Title}</h2>
                    <ul class="list-group">
                        <li class="text-primary list-group-item"><strong class="text-muted">Genre: </strong>${movie.Genre}</li>
                        <li class="text-primary list-group-item"><strong class="text-muted">Released: </strong>${movie.Released}</li>
                        <li class="text-primary list-group-item"><strong class="text-muted">Rated: </strong>${movie.Rated}</li>
                        <li class="text-primary list-group-item"><strong class="text-muted">IMDB Rating: </strong>${movie.imdbRating}</li>
                        <li class="text-primary list-group-item"><strong class="text-muted">Director: </strong>${movie.Director}</li>
                        <li class="text-primary list-group-item"><strong class="text-muted">Writer: </strong>${movie.Writer}</li>
                        <li class="text-primary list-group-item"><strong class="text-muted">Actors: </strong>${movie.Actors}</li>

                    </ul> 
                </div>
            </div>
            <div class="row text-primary">
                <div class="well m-5">
                    <h3>Plot</h3>
                    <p class="text-muted">${movie.Plot}</p>
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




// window.fbAsyncInit = function() {
//     FB.init({
//     appId      : '288034755543706',
//     cookie     : true,
//     xfbml      : true,
//     version    : 'v6.0'
//     });
//     FB.getLoginStatus(function(response) {
//         statusChangeCallback(response);
//     });   
// };

// (function(d, s, id){
//     var js, fjs = d.getElementsByTagName(s)[0];
//     if (d.getElementById(id)) {return;}
//     js = d.createElement(s); js.id = id;
//     js.src = "https://connect.facebook.net/en_US/sdk.js";
//     fjs.parentNode.insertBefore(js, fjs);
// }(document, 'script', 'facebook-jssdk'));

// function statusChangeCallback(response){
//     if(response.status==='connected'){
//         console.log("Logged in");
//         window.location="/success";
//     }
//     else{
//         console.log("not logged in");
//         window.location = "/login";

//     }
// }

// function checkLoginState() {
//     FB.getLoginStatus(function(response) {
//         statusChangeCallback(response);
//     });
// }
// function setElements(isLoggedIn){
    
// }


// function logout(){
//     FB.logout(function(response){
//         setElements(false);
//         window.location = "/login";
//     });
// }
