namespace ConwayGame.Tests.Utils;

public static class DataGenerator
{
    public static List<List<bool>> GenerateMatrix(int rows, int cols)
    {
        var random = new Random();
        var matrix = new List<List<bool>>();
        for (var i = 0; i < rows; i++)
        {
            var row = new List<bool>();
            for (var j = 0; j < cols; j++)
            {
                row.Add(random.Next(2) == 1);
            }

            matrix.Add(row);
        }

        return matrix;
    }

    public static IEnumerable<object[]> IncorrectMatrixElementCount()
    {
        yield return new object[]
        {
            new List<List<bool>>
            {
                new List<bool> { true, false, true, false },
                new List<bool> { false, true, false, false },
                new List<bool> { false, true },
                new List<bool> { true, false, true, false },
                new List<bool> { false, true, false },
                new List<bool> { false }
            }
        };
    }

    public static bool[] BlinkerArrayVertical =
    [
        false, false, false, false, false, false,
        false, false, true, false, false, false,
        false, false, true, false, false, false,
        false, false, true, false, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false
    ];

    public static bool[] BlinkerArrayHorizontal =
    [
        false, false, false, false, false, false,
        false, false, false, false, false, false,
        false, true, true, true, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false
    ];

    public static bool[] BeaconStep1 =
    [
        false, false, false, false, false, false,
        false, true, true, false, false, false,
        false, true, true, false, false, false,
        false, false, false, true, true, false,
        false, false, false, true, true, false,
        false, false, false, false, false, false
    ];

    public static bool[] BeaconStep2 =
    [
        false, false, false, false, false, false,
        false, true, true, false, false, false,
        false, true, false, false, false, false,
        false, false, false, false, true, false,
        false, false, false, true, true, false,
        false, false, false, false, false, false
    ];

    public static bool[] BlockStep1 =
    [
        false, false, false, false, false, false,
        false, true, true, false, false, false,
        false, true, false, false, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false
    ];

    public static bool[] BlockStep2 =
    [
        false, false, false, false, false, false,
        false, true, true, false, false, false,
        false, true, true, false, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false
    ];

    public static bool[] GliderStep1 =
    [
        false, false, false, false, false, false,
        false, false, false, true, false, false,
        false, true, false, true, false, false,
        false, false, true, true, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false
    ];
    
    public static bool[] GliderStep2 =
    [
        false, false, false, false, false, false,
        false, false, true, false, false, false,
        false, false, false, true, true, false,
        false, false, true, true, false, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false
    ];
    
    public static bool[] GliderStep3 =
    [
        false, false, false, false, false, false,
        false, false, false, true, false, false,
        false, false, false, false, true, false,
        false, false, true, true, true, false,
        false, false, false, false, false, false,
        false, false, false, false, false, false
    ];
    
    public static bool[] GliderStep4 =
    [
        false, false, false, false, false, false,
        false, false, false, false, false, false,
        false, false, true, false, true, false,
        false, false, false, true, true, false,
        false, false, false, true, false, false,
        false, false, false, false, false, false
    ];
    
    public static bool[] GliderStep5 =
    [
        false, false, false, false, false, false,
        false, false, false, false, false, false,
        false, false, false, false, true, false,
        false, false, true, false, true, false,
        false, false, false, true, true, false,
        false, false, false, false, false, false
    ];
}