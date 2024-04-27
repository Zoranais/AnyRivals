using AutoMapper;
using AnyRivals.Domain.Entities;

namespace AnyRivals.Application.Common.Models.DTOs;
public class PlayerDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Score { get; set; }

    private sealed class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Player, PlayerDto>();
        }
    }
}
