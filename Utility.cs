using System.Collections;

public class Utility  {

    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random rand = new System.Random(seed);
        for(int i = 0; i < array.Length; i++)
        {
            int randIndex = rand.Next(i, array.Length);
            T tempObject = array[i];
            array[i] = array[randIndex];
            array[randIndex] = tempObject;
        }
        return array;
    }
}
