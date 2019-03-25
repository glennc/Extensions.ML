
using System.Collections.Generic;

namespace BlazingMoviePredications
{
    public interface IMovieService
    {
        Movie Get(int id);
        IEnumerable<Movie> GetAllMovies();
        IEnumerable<Movie> GetRecentMovies();
        IEnumerable<Movie> GetSomeSuggestions();

        List<Movie> GetTrendingMovies { get; }
    }
}