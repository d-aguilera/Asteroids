namespace Asteroids.WinFormsApp
{
    using System;

    internal static class StatsContext
    {
        public static TimeSpan TimeGettingState;
        public static TimeSpan TimePreDraw;
        public static TimeSpan TimeDrawGrid;
        public static TimeSpan TimeDrawingAsteroids;
        public static TimeSpan TimeDrawingShip;
        public static TimeSpan TimeDrawingBullets;
        public static TimeSpan TimeDrawingHealthBar;
        public static TimeSpan TimeDrawingStats;
        public static TimeSpan TimeTotal;
        public static double TimeTickAvg;
        public static double Fps;
        public static int GcItemsCollected;

        public static void Collect(Telemetry telemetry)
        {
            TimeGettingState = telemetry.GetAverage("get-state");
            TimePreDraw = telemetry.GetAverage("pre-draw");
            TimeDrawGrid = telemetry.GetAverage("draw-grid");
            TimeDrawingAsteroids = telemetry.GetAverage("draw-asteroids");
            TimeDrawingShip = telemetry.GetAverage("draw-ship");
            TimeDrawingBullets = telemetry.GetAverage("draw-bullets");
            TimeDrawingHealthBar = telemetry.GetAverage("draw-health-bar");
            TimeDrawingStats = telemetry.GetAverage("draw-stats");
            TimeTotal = telemetry.GetAverage("total");
        }
    }
}
