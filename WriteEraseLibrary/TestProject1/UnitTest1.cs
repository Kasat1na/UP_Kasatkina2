using WriteEraseLibrary;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Test_NoFreeSlots()
        {
            TimeSpan[] startTimes = { TimeSpan.FromHours(8) };
            int[] durations = { 600 };
            TimeSpan beginWorkingTime = TimeSpan.FromHours(8);
            TimeSpan endWorkingTime = TimeSpan.FromHours(18);
            int consultationTime = 30;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);
            Assert.AreEqual(0, result.Length);
        }
       
        [TestMethod]
        public void Test_BusyPeriodsWithNoFreeTime_ReturnsEmptyArray()
        {
            // Arrange
            TimeSpan[] startTimes = { new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0) };
            int[] durations = { 60, 60 };
            TimeSpan beginWorkingTime = new TimeSpan(8, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(10, 0, 0);
            int consultationTime = 30;

            // Act
            var result = WriteEraseLibrary.Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void Test_FullDayBusyWithGapsSmallerThanConsultation()
        {
            TimeSpan[] startTimes = { TimeSpan.FromHours(8), TimeSpan.FromHours(9), TimeSpan.FromHours(10), TimeSpan.FromHours(11) };
            int[] durations = { 50, 50, 50, 50 };
            TimeSpan beginWorkingTime = TimeSpan.FromHours(8);
            TimeSpan endWorkingTime = TimeSpan.FromHours(12);
            int consultationTime = 60;

            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void Test_ComplexScheduleWithIrregularGaps()
        {
            // Arrange
            TimeSpan[] startTimes =
            {
                TimeSpan.FromHours(8),
                TimeSpan.FromHours(9.5),
                TimeSpan.FromHours(12),
                TimeSpan.FromHours(14.5),
                TimeSpan.FromHours(17)
            };
            int[] durations = { 30, 45, 90, 60, 60 };
            TimeSpan beginWorkingTime = TimeSpan.FromHours(8);
            TimeSpan endWorkingTime = TimeSpan.FromHours(18);
            int consultationTime = 30;

            // Act
            var result = WriteEraseLibrary.Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            string[] expectedSlots = {
                "08:30-09:00", "09:00-09:30",
                "10:15-10:45", "10:45-11:15", "11:15-11:45",
                "13:30-14:00", "14:00-14:30",
                "15:30-16:00", "16:00-16:30", "16:30-17:00"
            };

            CollectionAssert.AreEqual(expectedSlots, result);
        }
        [TestMethod]
        public void Test_NoBusyPeriods_ReturnsFullDaySlots()
        {
            // Arrange
            TimeSpan[] startTimes = new TimeSpan[0];
            int[] durations = new int[0];
            TimeSpan beginWorkingTime = new TimeSpan(8, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(18, 0, 0);
            int consultationTime = 30;

            // Act
            var result = WriteEraseLibrary.Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            Assert.IsTrue(result.Length == 20);
            Assert.IsTrue(result.All(slot => TimeSpan.Parse(slot.Split('-')[0]) >= beginWorkingTime && TimeSpan.Parse(slot.Split('-')[1]) <= endWorkingTime));
        }
        [TestMethod]
        public void Test_EmptyBusyPeriods_ReturnsFullDaySlots()
        {
            // Arrange
            TimeSpan[] startTimes = new TimeSpan[0];
            int[] durations = new int[0];
            TimeSpan beginWorkingTime = new TimeSpan(8, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(18, 0, 0);
            int consultationTime = 30;

            // Act
            var result = WriteEraseLibrary.Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            Assert.AreEqual(20, result.Length);
            Assert.AreEqual("08:00-08:30", result[0]);
            Assert.AreEqual("17:30-18:00", result[^1]);
        }
        [TestMethod]
        public void Test_SingleSlot()
        {
            // Arrange
            TimeSpan[] startTimes = { TimeSpan.FromHours(8) };
            int[] durations = { 30 };
            TimeSpan beginWorkingTime = TimeSpan.FromHours(8);
            TimeSpan endWorkingTime = TimeSpan.FromHours(18);
            int consultationTime = 30;

            // Act
            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            string[] expectedSlots = { "08:30-09:00" };
            CollectionAssert.AreNotEqual(expectedSlots, result);
        }

        [TestMethod]
        public void Test_LateEveningConsultationSlot()
        {
            // Arrange
            TimeSpan[] startTimes = { TimeSpan.FromHours(16) };
            int[] durations = { 120 };
            TimeSpan beginWorkingTime = TimeSpan.FromHours(8);
            TimeSpan endWorkingTime = TimeSpan.FromHours(18);
            int consultationTime = 30;

            // Act
            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            string[] expectedSlots = { "08:00-08:30", "08:30-09:00", "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "11:30-12:00", "12:00-12:30", "12:30-13:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00" };
            CollectionAssert.AreEqual(expectedSlots, result);
        }



        [TestMethod]
        public void Test_SingleFreeSlotAtStart()
        {
            // Arrange
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0) };
            int[] durations = { 60 };
            TimeSpan beginWorkingTime = new TimeSpan(8, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(18, 0, 0);
            int consultationTime = 30;

            // Act
            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            Assert.AreEqual(18, result.Length);
            Assert.IsTrue(result.Contains("08:00-08:30"));
        }

        [TestMethod]
        public void Test_OverlappingBusyPeriods()
        {
            // Arrange
            TimeSpan[] startTimes = { new TimeSpan(9, 0, 0), new TimeSpan(9, 15, 0) };
            int[] durations = { 30, 30 };
            TimeSpan beginWorkingTime = new TimeSpan(8, 0, 0);
            TimeSpan endWorkingTime = new TimeSpan(12, 0, 0);
            int consultationTime = 30;

            // Act
            var result = Calculations.AvailablePeriods(startTimes, durations, beginWorkingTime, endWorkingTime, consultationTime);

            // Assert
            Assert.AreEqual(6, result.Length);
            Assert.IsTrue(result.Contains("08:00-08:30"));
            Assert.IsTrue(result.Contains("08:30-09:00"));
        }


    }
}