using System;
using System.Collections.Generic;
using System.Linq;
using Bowling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        List<string> dataPoints = new List<string>();
        List<FrameScore> ballThrowList = new List<FrameScore>();
        List<int[]> frames = new List<int[]>();

        
        [TestMethod]
        public void TestScorePoints()
        {
            dataPoints.Add("[10,0]");
            dataPoints.Add("[7,3]");
            TrimData(dataPoints, frames, ballThrowList);

            //test the value in memory, if the trimdata method insert the right values
            Assert.AreEqual(10, ballThrowList[0].FirstThrow);
            Assert.AreEqual(0, ballThrowList[0].SecondThrow);

            Assert.AreEqual(7, ballThrowList[1].FirstThrow);
            Assert.AreEqual(3, ballThrowList[1].SecondThrow);

            dataPoints.Clear();

        }

        [TestMethod]
        public void TestSpareFrame()
        {
           
            dataPoints.Add("[7,3]");
            dataPoints.Add("[7,2]");
            dataPoints.Add("[10,0]");
            TrimData(dataPoints, frames, ballThrowList);

            //testing the throws to find if the  frame is Spare
             Assert.AreEqual(true, IsSpare(ballThrowList[0].FirstThrow, ballThrowList[0].SecondThrow));
             Assert.AreEqual(false, IsSpare(ballThrowList[1].FirstThrow, ballThrowList[1].SecondThrow));
             Assert.AreEqual(false, IsSpare(ballThrowList[2].FirstThrow, ballThrowList[2].SecondThrow));
             dataPoints.Clear();


        }

        [TestMethod]
        public void TestOpenFrame()
        {

            dataPoints.Add("[7,3]");
            dataPoints.Add("[7,2]");
            dataPoints.Add("[10,0]");
            TrimData(dataPoints, frames, ballThrowList);

            //testing the throws to find if the  frame is open
            Assert.AreEqual(false, IsOpenFrame(ballThrowList[0].FirstThrow, ballThrowList[0].SecondThrow));
            Assert.AreEqual(true, IsOpenFrame(ballThrowList[1].FirstThrow, ballThrowList[1].SecondThrow));
            Assert.AreEqual(false, IsOpenFrame(ballThrowList[2].FirstThrow, ballThrowList[2].SecondThrow));
            dataPoints.Clear();


        }

        [TestMethod]
        public void TestStrike()
        {

            dataPoints.Add("[7,3]");
            dataPoints.Add("[7,2]");
            dataPoints.Add("[10,0]");
            TrimData(dataPoints, frames, ballThrowList);

            //testing the throws to find if the  frame is a strike
            Assert.AreEqual(false, IsStrike(ballThrowList[0].FirstThrow));
            Assert.AreEqual(false, IsStrike(ballThrowList[1].FirstThrow));
            Assert.AreEqual(true, IsStrike(ballThrowList[2].FirstThrow));
            dataPoints.Clear();


        }
        [TestMethod]
        public void TestDoubleStrike()
        {

            dataPoints.Add("[7,3]");
            dataPoints.Add("[7,2]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            
         
            TrimData(dataPoints, frames, ballThrowList);

            //testing the throws to find if the  frame is a double strike
            
           //check if the previous first throw is not a strike expects false
            Assert.AreEqual(false, IsDoubleStrike(ballThrowList[2].FirstThrow, ballThrowList[1].FirstThrow));

            //check if the previous first throw is a strike expects true
            Assert.AreEqual(true, IsDoubleStrike(ballThrowList[3].FirstThrow,ballThrowList[2].FirstThrow));
            dataPoints.Clear();


        }

        //Since the higest point per.frame is 30 points by scoring 3 strikes in a roll, no need to check further
        [TestMethod]
        public void TestTripleStrike()
        {

            dataPoints.Add("[7,3]");
            dataPoints.Add("[7,2]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[7,2]");

            TrimData(dataPoints, frames, ballThrowList);

            
            Assert.AreEqual(false, IsTripleStrike(ballThrowList[6].FirstThrow,ballThrowList[5].FirstThrow, ballThrowList[7].FirstThrow));

           
            Assert.AreEqual(true, IsTripleStrike(ballThrowList[3].FirstThrow, ballThrowList[2].FirstThrow, ballThrowList[4].FirstThrow));
            dataPoints.Clear();


        }

        [TestMethod]
        public void TestSumScore()
        {

            dataPoints.Add("[7,3]");
            dataPoints.Add("[7,2]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[10,0]");
            dataPoints.Add("[7,2]");

            TrimData(dataPoints, frames, ballThrowList);
            List<int> testScoreList = new List<int>();
            testScoreList.Add(17);
            testScoreList.Add(26);
            testScoreList.Add(56);
            testScoreList.Add(86);
            testScoreList.Add(116);
            testScoreList.Add(143);
            testScoreList.Add(162);
            testScoreList.Add(171);
            List<int> testScoreListToTest = PostScore(ballThrowList);
            CollectionAssert.AreEqual(testScoreList, testScoreListToTest);
          

            dataPoints.Clear();


        }

        private static void TrimData(List<string> dataPoints, List<int[]> frames, List<FrameScore> ballThrowList)
        {
            foreach (var var in dataPoints)
            {
                string trimmed = var.TrimStart('[').TrimEnd(']');
                string[] split = trimmed.Split(',');
                int[] myInts = Array.ConvertAll(split, s => int.Parse(s));
                frames.Add(myInts);
            }

            foreach (var var in frames)
            {
                FrameScore thrown = new FrameScore();
                thrown.FirstThrow = var[0];
                thrown.SecondThrow = var[1];
                ballThrowList.Add(thrown);
            }
        }

        private bool IsSpare(int throw1, int throw2)
        {
            if (throw1 + throw2 == 10 && throw1 != 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsOpenFrame(int throw1, int throw2)
        {
            if (throw1 + throw2 < 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsStrike(int throw1)
        {
            if (throw1 == 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsDoubleStrike(int throw1, int previousFirstThrow)
        {
            if (IsStrike(throw1))
            {
                if (previousFirstThrow == 10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private bool IsTripleStrike(int throw1, int previousFirstThrow, int nextFirstThrow)
        {
            if (IsDoubleStrike(throw1, previousFirstThrow))
            {
                if (nextFirstThrow == 10 )
                {
                    return true;
                }
              
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private List<int> PostScore(List<FrameScore> ballThrowList)
        {
            int sum = 0;

            List<int> sumArray = new List<int>();
            for (int frame = 0; frame < ballThrowList.Count - 1; frame++)
            {
               
                int currentFrameScore = 0;

                if (IsOpenFrame(ballThrowList[frame].FirstThrow, ballThrowList[frame].SecondThrow))
                {
                    sum += ballThrowList[frame].FirstThrow +
                           ballThrowList[frame].SecondThrow;
                }
                
                else if (IsSpare(ballThrowList[frame].FirstThrow, ballThrowList[frame].SecondThrow))
                {
                    currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow;
                    sum += currentFrameScore;
                }
              
                else if (IsStrike(ballThrowList[frame].FirstThrow))
                {
                   
                    if (frame > 0 && IsDoubleStrike(ballThrowList[frame].FirstThrow,  ballThrowList[frame - 1].FirstThrow))
                    {

                        if ((ballThrowList.Count - 2) >= frame)
                        {
                            if (IsTripleStrike(ballThrowList[frame].FirstThrow, ballThrowList[frame - 1].FirstThrow,
                                    ballThrowList[frame + 1].FirstThrow) && ballThrowList[frame + 2].FirstThrow == 10)
                            {

                                currentFrameScore = 30;
                                sum += currentFrameScore;
                            }
                            else if (ballThrowList[frame + 1].FirstThrow == 10 &&
                                     ballThrowList[frame + 2].FirstThrow < 10)
                            {

                                currentFrameScore = 20 + ballThrowList[frame + 2].FirstThrow;
                                sum += currentFrameScore;
                            }

                            else
                            {

                                currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow +
                                                    ballThrowList[frame + 1].SecondThrow;
                                sum += currentFrameScore;
                            }
                        }
                        else
                        {
                            if (IsTripleStrike(ballThrowList[frame].FirstThrow, ballThrowList[frame - 1].FirstThrow,
                                    ballThrowList[frame + 1].FirstThrow) )
                            {

                                currentFrameScore = 30;
                                sum += currentFrameScore;
                            }
                         
                            else
                            {

                                currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow +
                                                    ballThrowList[frame + 1].SecondThrow;
                                sum += currentFrameScore;
                            }
                        }
                    }
                 
                    else
                    {
                     
                        if (ballThrowList[frame + 1].FirstThrow == 10)
                        {
                           
                            currentFrameScore = 20 + ballThrowList[frame + 2].FirstThrow;
                            sum += currentFrameScore;
                        }
                        else
                        {
                          
                         
                            currentFrameScore = 10 + ballThrowList[frame + 1].FirstThrow + ballThrowList[frame + 1].SecondThrow;
                            sum += currentFrameScore;
                        }

                    }

                }

             sumArray.Add(sum);
            }

            


            if (ballThrowList.Count == 11)
            {
                sum += ballThrowList[10].FirstThrow + ballThrowList[10].SecondThrow;
                sumArray.Add(sum);
                sumArray.RemoveAt(10);

            }
            else
            {
                sum += ballThrowList.Last().FirstThrow + ballThrowList.Last().SecondThrow;
                sumArray.Add(sum);

            }

       return sumArray;
        }
    }
}
