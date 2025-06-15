using System.ComponentModel.DataAnnotations;

namespace SuperSoccer.DomainLayer.Entities;

public record Player(
    string Name,
    short Height,
    short Weight);
    
