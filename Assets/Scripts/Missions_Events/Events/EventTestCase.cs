using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventTestCase
{
    #region Fields
    private string testName;

    /*  Testing atributes */
    private bool isTestingLevel;
    private bool isTestingSocial;
    private bool isTestingTechnical;
    private bool isTestingScience;
    private bool isTestingMilitary;
    private bool isSpecialTest;

    private int testDiff;
    private int testRateMod;
    private string priority;

    private bool karmaInfluence; // ToDO Karma is not finish yet

    //public string karmaDescription;

    private int minimalCharacterParticipation;
    private int maximalCharacterParticipation;
    TestType testType;
    ClassTest classTest;
    TestingSubjects subject;

    #endregion
    // karmaDescription , Todo
    #region Constructor 

    public EventTestCase()
    {

    }
    public EventTestCase(string _testTarget, string _testName, string _kindTest, string _testType,
        string _testAtribute, int _testDiff, int _testRate, int _karmaInfluence, string _karmaDescription,
        int _min, int _max, string _resultPriority)
    {
        testType = (TestType)Enum.Parse(typeof(TestType), _testType, true);
        minimalCharacterParticipation = _min;
        maximalCharacterParticipation = _max;
        karmaInfluence = Convert.ToBoolean(_karmaInfluence);
        testRateMod = _testRate;
        testDiff = _testDiff;
        classTest = (ClassTest)Enum.Parse(typeof(ClassTest), _kindTest, true);
        subject = (TestingSubjects)Enum.Parse(typeof(TestingSubjects), _testTarget, true);
        ChooseTestingSkills(_testAtribute);
        testName = _testName;
        priority = _resultPriority;
    }

    public EventTestCase(StatsClass statClass)
    {
        string testTarget = statClass.GetStrStat("TestTarget");

        testType = (TestType)Enum.Parse(typeof(TestType), statClass.GetStrStat("TestType"), true);
        minimalCharacterParticipation = statClass.GetIntStat("SpecTestNumMin");
        maximalCharacterParticipation = statClass.GetIntStat("SpecTestNumMax");
        karmaInfluence = Convert.ToBoolean(statClass.GetIntStat("KarmaInfluence"));
        testRateMod = statClass.GetIntStat("TestRateMod");
        testDiff = statClass.GetIntStat("TestDiff");
        classTest = (ClassTest)Enum.Parse(typeof(ClassTest), statClass.GetStrStat("KindTest"), true);
        subject = (TestingSubjects)Enum.Parse(typeof(TestingSubjects), testTarget, true);
        ChooseTestingSkills(statClass.GetStrStat("TestAtribute"));
        testName = statClass.GetStrStat("TestName");
        priority = statClass.GetStrStat("ResultPriority");
    }

    #endregion
    #region Properities
    public int GetMaxCharParticipation { get { return this.maximalCharacterParticipation; } }
    public int GetMinCharParticipation { get { return this.minimalCharacterParticipation; } }
    public int GetRateMod { get { return this.testRateMod; } }
    public int GetDifficulty { get { return this.testDiff; } }
    public bool GetKarmaInfluence { get { return this.karmaInfluence; } }
    public TestType GetType { get { return this.testType; } }
    public ClassTest GetClass { get { return this.classTest; } }
    public TestingSubjects GetTestingSubjects { get { return this.subject; } }
    public string GetName { get { return this.testName; } }
    public bool IsTestingLevel { get { return this.isTestingLevel; } }
    public bool IsTestingMilitary { get { return this.isTestingMilitary; } }
    public bool IsTestingSocial { get { return this.isTestingSocial; } }
    public bool IsTestingScience { get { return this.isTestingScience; } }
    public bool IsTestingTechnical { get { return this.isTestingTechnical; } }
    public string GetPriority { get { return this.priority; } }

    #endregion

    public void ChooseTestingSkills(string _testAtribute)
    {
        isTestingLevel = _testAtribute.Contains("LvL");
        isTestingMilitary = _testAtribute.Contains("MiL");
        isTestingScience = _testAtribute.Contains("ScL");
        isTestingSocial = _testAtribute.Contains("SoL");
        isTestingTechnical = _testAtribute.Contains("TeL");
        isSpecialTest = _testAtribute.Contains("Other");
    }

    public string ReturnTestingAtribute()
    {
        string testingSkill = "Error";

        if (isTestingLevel)
            testingSkill = "Level";
        if (isTestingMilitary)
            testingSkill = "Military";
        if (isTestingScience)
            testingSkill = "Science";
        if (isTestingSocial)
            testingSkill = "Social";
        if (isTestingTechnical)
            testingSkill = "Technical";
        if(isSpecialTest)
            testingSkill = "Special_Condition";

        return testingSkill;
    }


    public enum TestType
    {
        Battle,
        Comunication,
        DiggBuild,
        Gathering,
        Hunting,
        Leverage,
        LockPicking,
        Repair,
        Research,
        Scavenging,
        Scouting,
        SelectOnly,
        Sneaking,
    }

    public enum ClassTest
    {
        Together,
        Separately
    }

    public enum TestingSubjects
    {
        RangeNum,
        SpecActive,
        RangeNumRng,
        DirectNum,
        DirectNumRng,
        AllGroup
    }

}
