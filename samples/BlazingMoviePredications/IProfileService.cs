using System.Collections.Generic;

namespace BlazingMoviePredications
{
    public interface IProfileService
    {
        Profile GetProfileByID(int id);

        List<(int movieId, int movieRating)> GetProfileWatchedMovies(int id);

        List<Profile> Profiles { get; }
    }
}