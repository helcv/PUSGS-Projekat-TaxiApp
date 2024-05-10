
using AutoMapper;

namespace Taxi_App;

public class RatingRepository : IRatingRepository
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;

    public RatingRepository(IMapper mapper, DataContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    public void AddRating(Rating rating)
    {
        _context.Ratings.Add(rating);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
