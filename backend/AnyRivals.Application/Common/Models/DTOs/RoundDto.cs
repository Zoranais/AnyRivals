using AutoMapper;
using AnyRivals.Domain.Entities;
using AnyRivals.Domain.Enums;

namespace AnyRivals.Application.Common.Models.DTOs;
public class RoundDto
{
    public ICollection<string> Answers { get; set; }

    public QuestionType QuestionType { get; set; }

    public string Source { get; set; }

    public string? Title { get; set; }

    private sealed class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Question, RoundDto>()
                .ForMember(x => x.Answers, x => x.MapFrom(x => x.AvailableAnswers.Select(x => x.Text)))
                .ForMember(x => x.QuestionType, x => x.MapFrom(x => x.Type));
        }
    }
}
