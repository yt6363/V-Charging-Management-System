using System;

namespace Evo.StaticFunctions {
  internal class Priority {
    public readonly record struct TimeSlot(int Id, string Slot);

    public static TimeSlot GenerateTimeSlot(DateTime time) {
      // Duration of one time slot
      const int delta = 10;

      // Get minute and hour from the time
      int hour = time.Hour;
      int minute = time.Minute;

      // Drop the ones place
      minute = ((int)minute / delta) * delta;

      // Assign each a slot unique Id which is max at 00:00 and min at 23:59
      int id = (60 / delta) * (23 - hour) + (60 / delta) - ((int)minute / delta);
      string slot = $"{hour:00}:{minute:00} - {hour:00}:{minute + 9:00}";
      return new TimeSlot(id, slot);
    }

    public static double GeneratePriority(int soc, double fee) {
      // Priority modifiers
      const double kSoc = 0.005; // [0 - 0.5]
      const double kFee = 0.01; // [0 - 0.2]

      // Apply modifiers
      double socPriority = 0.5 - kSoc * soc;
      double feePriority = kFee * fee;
      if (feePriority > 0.2) { feePriority = 0.2; }

      return socPriority + feePriority;
    }
  }
}
