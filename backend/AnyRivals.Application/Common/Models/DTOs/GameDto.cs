using AutoMapper;
using AnyRivals.Domain.Entities;
using AnyRivals.Domain.Enums;

namespace AnyRivals.Application.Common.Models.DTOs;
public class GameDto
{
    public int Id { get; set; }

    public string ExternalId { get; set; }

    public string Name { get; set; }

    public bool RequirePassword { get; set; }

    public int RoundCount { get; set; }

    public GameState State { get; set; }

    public ICollection<PlayerDto> Players { get; set; } = [];

    private sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Game, GameDto>()
                .ForMember(x => x.RoundCount, x => x.MapFrom(x => x.TotalRounds))
                .ForMember(x => x.RequirePassword, x => x.MapFrom(x => string.IsNullOrWhiteSpace(x.GamePassword)));
        }
    }
}
