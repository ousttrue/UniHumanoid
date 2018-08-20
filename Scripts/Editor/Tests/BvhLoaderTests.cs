using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace UniHumanoid
{
    public class BvhLoaderTests
    {
        /// <summary>
        /// https://github.com/wspr/bvh-matlab/blob/master/louise.bvh
        /// </summary>
        [Test]
        public void GuessBoneMappingTest()
        {
            var src = @"HIERARCHY
ROOT Hips
{
    OFFSET 0.000000 0.000000 0.000000
    CHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation
        JOINT Chest
        {
            OFFSET -0.000000 30.833075 -0.000000
            CHANNELS 3 Zrotation Xrotation Yrotation
            JOINT Neck
            {
                OFFSET -0.000000 23.115997 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT Head
                {
                    OFFSET -0.000000 10.266666 0.000000
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    End Site
                    {
                        OFFSET -0.000000 15.866669 0.000000
                    }
                }
            }
            JOINT LeftCollar
            {
                OFFSET -0.000000 23.115997 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT LeftShoulder
                {
                    OFFSET 18.666668 -0.000000 0.000000
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    JOINT LeftElbow
                    {
                        OFFSET 25.298601 0.000000 0.000000
                        CHANNELS 3 Zrotation Xrotation Yrotation
                        JOINT LeftWrist
                        {
                            OFFSET 27.056377 0.000000 0.000000
                            CHANNELS 3 Zrotation Xrotation Yrotation
                            End Site
                            {
                                OFFSET 0.000000 -14.000002 0.000000
                            }
                        }
                    }
                }
            }
            JOINT RightCollar
            {
                OFFSET -0.000000 23.115997 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT RightShoulder
                {
                    OFFSET -18.666668 0.000000 0.000000
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    JOINT RightElbow
                    {
                        OFFSET -25.298601 0.000000 0.000000
                        CHANNELS 3 Zrotation Xrotation Yrotation
                        JOINT RightWrist
                        {
                            OFFSET -27.056377 0.000000 0.000000
                            CHANNELS 3 Zrotation Xrotation Yrotation
                            End Site
                            {
                                OFFSET -0.000000 -14.000002 0.000000
                            }
                        }
                    }
                }
            }
        }
    JOINT LeftHip
    {
        OFFSET 11.200000 0.000000 0.000000
        CHANNELS 3 Zrotation Xrotation Yrotation
        JOINT LeftKnee
        {
            OFFSET -0.000000 -43.871983 0.000000
            CHANNELS 3 Zrotation Xrotation Yrotation
            JOINT LeftAnkle
            {
                OFFSET -0.000000 -44.488350 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                End Site
                {
                    OFFSET -0.000000 -4.666667 15.866669
                }
            }
        }
    }
    JOINT RightHip
    {
        OFFSET -11.200000 0.000000 0.000000
        CHANNELS 3 Zrotation Xrotation Yrotation
        JOINT RightKnee
        {
            OFFSET -0.000000 -43.871983 0.000000
            CHANNELS 3 Zrotation Xrotation Yrotation
            JOINT RightAnkle
            {
                OFFSET -0.000000 -44.488350 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                End Site
                {
                    OFFSET -0.000000 -4.666667 15.866669
                }
            }
        }
    }
}
";
            var bvh = Bvh.Parse(src);
            var detector = new BvhSkeletonEstimator();
            var skeleton = detector.Detect(bvh);

            Assert.AreEqual(0, skeleton.GetBoneIndex(HumanBodyBones.Hips));
            Assert.AreEqual("Hips", skeleton.GetBoneName(HumanBodyBones.Hips));
            Assert.AreEqual("Chest", skeleton.GetBoneName(HumanBodyBones.Chest));
            //Assert.AreEqual("Neck", skeleton.GetBoneName(HumanBodyBones.Neck));
            //Assert.AreEqual("Head", skeleton.GetBoneName(HumanBodyBones.Head));
        }
    }
}
