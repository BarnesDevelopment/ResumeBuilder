namespace ResumeAPI.Helpers;

public class TiltedException : Exception
{
  public TiltedException() : base("How did you get here? TILT")
  {
  }
}
