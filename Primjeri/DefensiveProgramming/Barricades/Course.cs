using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Barricades
{
  class Course
  {
    public string Name { get; private set; }

    //Consider why is dictionary private variable, and not property!    
    //indexer serves as a barricade
    
    private Dictionary<string, int> grades = new Dictionary<string, int>();
    public Course(string name)
    {
      Name = name;
    }

    /// <summary>
    /// Assign the new grade to the student.
    /// Old value (if had existed) is replaced.    
    /// </summary>
    /// <param name="name">Student's name</param>
    /// <returns>The student's grade or -1 if the student does not have a grade</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown if value is not valid grade [1-5]</exception>
    public int this[string name]
    {
      get
      {
        //what if there is no student's name in the dictionary
        //throw exception, return special value? See error handling techniques
        bool exists = grades.TryGetValue(name, out int grade);
        return exists ? grade : -1;
      }
      set
      {
        if (value < 1 || value > 5)
        {
          throw new ArgumentOutOfRangeException($"Invalid grade {value}. It shoud be from 1 to 5");
        }

        grades[name] = value;
      }
    }

    public double AverageGrade()
    {
      int sum = 0;
      foreach (var pair in grades)
      {
        int grade = pair.Value;
        Debug.Assert(grade >= 1 && grade <= 5,
                     $"Invalid grade in dictionary {pair.Key} = {pair.Value}. This should never happens!");
        sum += grade;
      }
      return grades.Count > 0 ? (double)sum / grades.Count : 0;
    }
  }
}