namespace Nerd.Domain.Utillities;

public static class GuidUtility
{
    public static Guid GenerateSemiGuid(this Guid guid) => GenerateGuid(DateTime.UtcNow);

    public static Guid GenerateSemiGuid() => GenerateGuid(DateTime.UtcNow);

    public static Guid GenerateGuid(in DateTime time)
    {
        Span<byte> newGuid = stackalloc byte[16];
        Span<byte> ticks = BitConverter.GetBytes(time.Ticks);

        ticks.CopyTo(newGuid[8..]);

        Random.Shared.NextBytes(newGuid[..8]);

        return new Guid(newGuid);
    }
}
