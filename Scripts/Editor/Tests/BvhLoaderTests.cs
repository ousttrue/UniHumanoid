using NUnit.Framework;
using UnityEngine;


namespace UniHumanoid
{
    public class BvhLoaderTests
    {
        #region LOUICE
        /// <summary>
        /// https://github.com/wspr/bvh-matlab/blob/master/louise.bvh
        /// </summary>
        const string bvh_louise = @"HIERARCHY
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

        [Test]
        public void GuessBoneMapping_louise()
        {
            var bvh = Bvh.Parse(bvh_louise);
            var detector = new BvhSkeletonEstimator();
            var skeleton = detector.Detect(bvh);

            Assert.AreEqual(0, skeleton.GetBoneIndex(HumanBodyBones.Hips));

            Assert.AreEqual("Hips", skeleton.GetBoneName(HumanBodyBones.Hips));
            Assert.AreEqual("Chest", skeleton.GetBoneName(HumanBodyBones.Spine));
            Assert.IsNull(skeleton.GetBoneName(HumanBodyBones.Chest));
            Assert.AreEqual("Neck", skeleton.GetBoneName(HumanBodyBones.Neck));
            Assert.AreEqual("Head", skeleton.GetBoneName(HumanBodyBones.Head));

            Assert.AreEqual("LeftCollar", skeleton.GetBoneName(HumanBodyBones.LeftShoulder));
            Assert.AreEqual("LeftShoulder", skeleton.GetBoneName(HumanBodyBones.LeftUpperArm));
            Assert.AreEqual("LeftElbow", skeleton.GetBoneName(HumanBodyBones.LeftLowerArm));
            Assert.AreEqual("LeftWrist", skeleton.GetBoneName(HumanBodyBones.LeftHand));

            Assert.AreEqual("RightCollar", skeleton.GetBoneName(HumanBodyBones.RightShoulder));
            Assert.AreEqual("RightShoulder", skeleton.GetBoneName(HumanBodyBones.RightUpperArm));
            Assert.AreEqual("RightElbow", skeleton.GetBoneName(HumanBodyBones.RightLowerArm));
            Assert.AreEqual("RightWrist", skeleton.GetBoneName(HumanBodyBones.RightHand));

            Assert.AreEqual("LeftHip", skeleton.GetBoneName(HumanBodyBones.LeftUpperLeg));
            Assert.AreEqual("LeftKnee", skeleton.GetBoneName(HumanBodyBones.LeftLowerLeg));
            Assert.AreEqual("LeftAnkle", skeleton.GetBoneName(HumanBodyBones.LeftFoot));
            Assert.IsNull(skeleton.GetBoneName(HumanBodyBones.LeftToes));

            Assert.AreEqual("RightHip", skeleton.GetBoneName(HumanBodyBones.RightUpperLeg));
            Assert.AreEqual("RightKnee", skeleton.GetBoneName(HumanBodyBones.RightLowerLeg));
            Assert.AreEqual("RightAnkle", skeleton.GetBoneName(HumanBodyBones.RightFoot));
            Assert.IsNull(skeleton.GetBoneName(HumanBodyBones.RightToes));
        }
        #endregion

        #region cgspeed
        /// <summary>
        /// https://sites.google.com/a/cgspeed.com/cgspeed/motion-capture
        /// </summary>
        const string bvh_cgspeed = @"HIERARCHY
ROOT Hips
{
	OFFSET 0.00000 0.00000 0.00000
	CHANNELS 6 Xposition Yposition Zposition Zrotation Yrotation Xrotation 
	JOINT LHipJoint
	{
		OFFSET 0 0 0
		CHANNELS 3 Zrotation Yrotation Xrotation
		JOINT LeftUpLeg
		{
			OFFSET 1.64549 -1.70879 0.84566
			CHANNELS 3 Zrotation Yrotation Xrotation
			JOINT LeftLeg
			{
				OFFSET 2.24963 -6.18082 0.00000
				CHANNELS 3 Zrotation Yrotation Xrotation
				JOINT LeftFoot
				{
					OFFSET 2.71775 -7.46697 0.00000
					CHANNELS 3 Zrotation Yrotation Xrotation
					JOINT LeftToeBase
					{
						OFFSET 0.18768 -0.51564 2.24737
						CHANNELS 3 Zrotation Yrotation Xrotation
						End Site
						{
							OFFSET 0.00000 -0.00000 1.15935
						}
					}
				}
			}
		}
	}
	JOINT RHipJoint
	{
		OFFSET 0 0 0
		CHANNELS 3 Zrotation Yrotation Xrotation
		JOINT RightUpLeg
		{
			OFFSET -1.58830 -1.70879 0.84566
			CHANNELS 3 Zrotation Yrotation Xrotation
			JOINT RightLeg
			{
				OFFSET -2.25006 -6.18201 0.00000
				CHANNELS 3 Zrotation Yrotation Xrotation
				JOINT RightFoot
				{
					OFFSET -2.72829 -7.49593 0.00000
					CHANNELS 3 Zrotation Yrotation Xrotation
					JOINT RightToeBase
					{
						OFFSET -0.21541 -0.59185 2.10643
						CHANNELS 3 Zrotation Yrotation Xrotation
						End Site
						{
							OFFSET -0.00000 -0.00000 1.09838
						}
					}
				}
			}
		}
	}
	JOINT LowerBack
	{
		OFFSET 0 0 0
		CHANNELS 3 Zrotation Yrotation Xrotation
		JOINT Spine
		{
			OFFSET 0.03142 2.10496 -0.11038
			CHANNELS 3 Zrotation Yrotation Xrotation
			JOINT Spine1
			{
				OFFSET -0.01863 2.10897 -0.06956
				CHANNELS 3 Zrotation Yrotation Xrotation
				JOINT Neck
				{
					OFFSET 0 0 0
					CHANNELS 3 Zrotation Yrotation Xrotation
					JOINT Neck1
					{
						OFFSET -0.02267 1.73238 0.00451
						CHANNELS 3 Zrotation Yrotation Xrotation
						JOINT Head
						{
							OFFSET -0.05808 1.54724 -0.61749
							CHANNELS 3 Zrotation Yrotation Xrotation
							End Site
							{
								OFFSET -0.01396 1.71468 -0.21082
							}
						}
					}
				}
				JOINT LeftShoulder
				{
					OFFSET 0 0 0
					CHANNELS 3 Zrotation Yrotation Xrotation
					JOINT LeftArm
					{
						OFFSET 3.44898 0.50298 0.21920
						CHANNELS 3 Zrotation Yrotation Xrotation
						JOINT LeftForeArm
						{
							OFFSET 5.41917 -0.00000 -0.00000
							CHANNELS 3 Zrotation Yrotation Xrotation
							JOINT LeftHand
							{
								OFFSET 2.44373 -0.00000 0.00000
								CHANNELS 3 Zrotation Yrotation Xrotation
								JOINT LeftFingerBase
								{
									OFFSET 0 0 0
									CHANNELS 3 Zrotation Yrotation Xrotation
									JOINT LeftHandIndex1
									{
										OFFSET 0.72750 -0.00000 0.00000
										CHANNELS 3 Zrotation Yrotation Xrotation
										End Site
										{
											OFFSET 0.58653 -0.00000 0.00000
										}
									}
								}
								JOINT LThumb
								{
									OFFSET 0 0 0
									CHANNELS 3 Zrotation Yrotation Xrotation
									End Site
									{
										OFFSET 0.59549 -0.00000 0.59549
									}
								}
							}
						}
					}
				}
				JOINT RightShoulder
				{
					OFFSET 0 0 0
					CHANNELS 3 Zrotation Yrotation Xrotation
					JOINT RightArm
					{
						OFFSET -3.23015 0.55830 0.31051
						CHANNELS 3 Zrotation Yrotation Xrotation
						JOINT RightForeArm
						{
							OFFSET -5.58976 -0.00010 0.00014
							CHANNELS 3 Zrotation Yrotation Xrotation
							JOINT RightHand
							{
								OFFSET -2.48060 -0.00000 0.00000
								CHANNELS 3 Zrotation Yrotation Xrotation
								JOINT RightFingerBase
								{
									OFFSET 0 0 0
									CHANNELS 3 Zrotation Yrotation Xrotation
									JOINT RightHandIndex1
									{
										OFFSET -0.81601 -0.00000 0.00000
										CHANNELS 3 Zrotation Yrotation Xrotation
										End Site
										{
											OFFSET -0.65789 -0.00000 0.00000
										}
									}
								}
								JOINT RThumb
								{
									OFFSET 0 0 0
									CHANNELS 3 Zrotation Yrotation Xrotation
									End Site
									{
										OFFSET -0.66793 -0.00000 0.66793
									}
								}
							}
						}
					}
				}
			}
		}
	}
}"
;
        [Test]
        public void GuessBoneMapping_cgspeed()
        {
            var bvh = Bvh.Parse(bvh_cgspeed);
            var detector = new BvhSkeletonEstimator();
            var skeleton = detector.Detect(bvh);

            Assert.AreEqual(0, skeleton.GetBoneIndex(HumanBodyBones.Hips));

            Assert.AreEqual("Hips", skeleton.GetBoneName(HumanBodyBones.Hips));
            Assert.AreEqual("LowerBack", skeleton.GetBoneName(HumanBodyBones.Spine));
            Assert.AreEqual("Spine", skeleton.GetBoneName(HumanBodyBones.Chest));
            Assert.AreEqual("Spine1", skeleton.GetBoneName(HumanBodyBones.UpperChest));

            Assert.AreEqual("Neck", skeleton.GetBoneName(HumanBodyBones.Neck));
            Assert.AreEqual("Head", skeleton.GetBoneName(HumanBodyBones.Head));

            Assert.AreEqual("LeftShoulder", skeleton.GetBoneName(HumanBodyBones.LeftShoulder));
            Assert.AreEqual("LeftArm", skeleton.GetBoneName(HumanBodyBones.LeftUpperArm));
            Assert.AreEqual("LeftForeArm", skeleton.GetBoneName(HumanBodyBones.LeftLowerArm));
            Assert.AreEqual("LeftHand", skeleton.GetBoneName(HumanBodyBones.LeftHand));

            Assert.AreEqual("RightShoulder", skeleton.GetBoneName(HumanBodyBones.RightShoulder));
            Assert.AreEqual("RightArm", skeleton.GetBoneName(HumanBodyBones.RightUpperArm));
            Assert.AreEqual("RightForeArm", skeleton.GetBoneName(HumanBodyBones.RightLowerArm));
            Assert.AreEqual("RightHand", skeleton.GetBoneName(HumanBodyBones.RightHand));

            Assert.AreEqual("LeftUpLeg", skeleton.GetBoneName(HumanBodyBones.LeftUpperLeg));
            Assert.AreEqual("LeftLeg", skeleton.GetBoneName(HumanBodyBones.LeftLowerLeg));
            Assert.AreEqual("LeftFoot", skeleton.GetBoneName(HumanBodyBones.LeftFoot));
            Assert.AreEqual("LeftToeBase", skeleton.GetBoneName(HumanBodyBones.LeftToes));

            Assert.AreEqual("RightUpLeg", skeleton.GetBoneName(HumanBodyBones.RightUpperLeg));
            Assert.AreEqual("RightLeg", skeleton.GetBoneName(HumanBodyBones.RightLowerLeg));
            Assert.AreEqual("RightFoot", skeleton.GetBoneName(HumanBodyBones.RightFoot));
            Assert.AreEqual("RightToeBase", skeleton.GetBoneName(HumanBodyBones.RightToes));
        }
        #endregion

        #region mocap
        const string bvh_mocap = @"HIERARCHY
ROOT Hips
{
    OFFSET 0.000000 0.000000 0.000000
    CHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation
    JOINT Spine
    {
        OFFSET -0.000000 7.509519 0.000000
        CHANNELS 3 Zrotation Xrotation Yrotation
        JOINT Spine1
        {
            OFFSET -0.000000 18.393364 0.000000
            CHANNELS 3 Zrotation Xrotation Yrotation
            JOINT Neck
            {
                OFFSET -0.000000 20.224955 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT Head
                {
                    OFFSET -0.000000 14.194822 1.831590
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    End Site
                    {
                        OFFSET -0.000000 18.315899 0.000000
                    }
                }
            }
            JOINT LeftShoulder
            {
                OFFSET 3.663486 15.569419 -0.490481
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT LeftArm
                {
                    OFFSET 14.246625 0.000000 0.000000
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    JOINT LeftForeArm
                    {
                        OFFSET 25.567986 0.000000 0.000000
                        CHANNELS 3 Zrotation Xrotation Yrotation
                        JOINT LeftHand
                        {
                            OFFSET 29.965693 0.000000 0.000000
                            CHANNELS 3 Zrotation Xrotation Yrotation
                            End Site
                            {
                                OFFSET 13.736924 0.000000 0.000000
                            }
                        }
                    }
                }
            }
            JOINT RightShoulder
            {
                OFFSET -3.661042 15.569419 -0.490481
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT RightArm
                {
                    OFFSET -14.246625 0.000000 0.000000
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    JOINT RightForeArm
                    {
                        OFFSET -25.567986 0.000000 0.000000
                        CHANNELS 3 Zrotation Xrotation Yrotation
                        JOINT RightHand
                        {
                            OFFSET -29.965693 0.000000 0.000000
                            CHANNELS 3 Zrotation Xrotation Yrotation
                            End Site
                            {
                                OFFSET -13.736924 0.000000 0.000000
                            }
                        }
                    }
                }
            }
        }
    }
    JOINT LeftUpLeg
    {
        OFFSET 9.157949 0.000000 0.000000
        CHANNELS 3 Zrotation Xrotation Yrotation
        JOINT LeftLeg
        {
            OFFSET -0.000000 -40.189121 0.000000
            CHANNELS 3 Zrotation Xrotation Yrotation
            JOINT LeftFoot
            {
                OFFSET -0.000000 -39.816978 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT LeftToeBase
                {
                    OFFSET -0.000000 -5.952667 13.736924
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    End Site
                    {
                        OFFSET -0.000000 0.000000 3.663180
                    }
                }
            }
        }
    }
    JOINT RightUpLeg
    {
        OFFSET -9.157949 0.000000 0.000000
        CHANNELS 3 Zrotation Xrotation Yrotation
        JOINT RightLeg
        {
            OFFSET -0.000000 -40.189121 0.000000
            CHANNELS 3 Zrotation Xrotation Yrotation
            JOINT RightFoot
            {
                OFFSET -0.000000 -39.816978 0.000000
                CHANNELS 3 Zrotation Xrotation Yrotation
                JOINT RightToeBase
                {
                    OFFSET -0.000000 -5.952667 13.736924
                    CHANNELS 3 Zrotation Xrotation Yrotation
                    End Site
                    {
                        OFFSET -0.000000 0.000000 3.663180
                    }
                }
            }
        }
    }
}
";

        [Test]
        public void GuessBoneMapping_mocap()
        {
            var bvh = Bvh.Parse(bvh_mocap);
            var detector = new BvhSkeletonEstimator();
            var skeleton = detector.Detect(bvh);

            Assert.AreEqual(0, skeleton.GetBoneIndex(HumanBodyBones.Hips));

            Assert.AreEqual("Hips", skeleton.GetBoneName(HumanBodyBones.Hips));
            Assert.AreEqual("Spine", skeleton.GetBoneName(HumanBodyBones.Spine));
            Assert.AreEqual("Spine1", skeleton.GetBoneName(HumanBodyBones.Chest));
            Assert.AreEqual(null, skeleton.GetBoneName(HumanBodyBones.UpperChest));

            Assert.AreEqual("Neck", skeleton.GetBoneName(HumanBodyBones.Neck));
            Assert.AreEqual("Head", skeleton.GetBoneName(HumanBodyBones.Head));

            Assert.AreEqual("LeftShoulder", skeleton.GetBoneName(HumanBodyBones.LeftShoulder));
            Assert.AreEqual("LeftArm", skeleton.GetBoneName(HumanBodyBones.LeftUpperArm));
            Assert.AreEqual("LeftForeArm", skeleton.GetBoneName(HumanBodyBones.LeftLowerArm));
            Assert.AreEqual("LeftHand", skeleton.GetBoneName(HumanBodyBones.LeftHand));

            Assert.AreEqual("RightShoulder", skeleton.GetBoneName(HumanBodyBones.RightShoulder));
            Assert.AreEqual("RightArm", skeleton.GetBoneName(HumanBodyBones.RightUpperArm));
            Assert.AreEqual("RightForeArm", skeleton.GetBoneName(HumanBodyBones.RightLowerArm));
            Assert.AreEqual("RightHand", skeleton.GetBoneName(HumanBodyBones.RightHand));

            Assert.AreEqual("LeftUpLeg", skeleton.GetBoneName(HumanBodyBones.LeftUpperLeg));
            Assert.AreEqual("LeftLeg", skeleton.GetBoneName(HumanBodyBones.LeftLowerLeg));
            Assert.AreEqual("LeftFoot", skeleton.GetBoneName(HumanBodyBones.LeftFoot));
            Assert.AreEqual("LeftToeBase", skeleton.GetBoneName(HumanBodyBones.LeftToes));

            Assert.AreEqual("RightUpLeg", skeleton.GetBoneName(HumanBodyBones.RightUpperLeg));
            Assert.AreEqual("RightLeg", skeleton.GetBoneName(HumanBodyBones.RightLowerLeg));
            Assert.AreEqual("RightFoot", skeleton.GetBoneName(HumanBodyBones.RightFoot));
            Assert.AreEqual("RightToeBase", skeleton.GetBoneName(HumanBodyBones.RightToes));
        }
        #endregion

        #region mocap2
        const string bvh_mocap2 = @"HIERARCHY
ROOT Hips
{
 OFFSET 0.000000 0.000000 0.000000
 CHANNELS 6 Xposition Yposition Zposition Yrotation Xrotation Zrotation
 JOINT Chest
 {
  OFFSET 0.000000 10.678932 0.006280
  CHANNELS 3 Yrotation Xrotation Zrotation
  JOINT Chest2
  {
   OFFSET 0.000000 10.491159 -0.011408
   CHANNELS 3 Yrotation Xrotation Zrotation
   JOINT Chest3
   {
    OFFSET 0.000000 9.479342 0.000000
    CHANNELS 3 Yrotation Xrotation Zrotation
    JOINT Chest4
    {
     OFFSET 0.000000 9.479342 0.000000
     CHANNELS 3 Yrotation Xrotation Zrotation
     JOINT Neck
     {
      OFFSET 0.000000 13.535332 0.000000
      CHANNELS 3 Yrotation Xrotation Zrotation
      JOINT Head
      {
       OFFSET 0.000000 8.819083 -0.027129
       CHANNELS 3 Yrotation Xrotation Zrotation
       End Site
       {
        OFFSET 0.000000 16.966594 -0.014170
       }
      }
     }
     JOINT RightCollar
     {
      OFFSET -3.012546 7.545150 0.000000
      CHANNELS 3 Yrotation Xrotation Zrotation
      JOINT RightShoulder
      {
       OFFSET -13.683099 0.000000 0.000000
       CHANNELS 3 Yrotation Xrotation Zrotation
       JOINT RightElbow
       {
        OFFSET -26.359998 0.000000 0.000000
        CHANNELS 3 Yrotation Xrotation Zrotation
        JOINT RightWrist
        {
         OFFSET -21.746691 0.000000 0.008601
         CHANNELS 3 Yrotation Xrotation Zrotation
         End Site
         {
          OFFSET -16.348058 0.000000 0.000000
         }
        }
       }
      }
     }
     JOINT LeftCollar
     {
      OFFSET 3.012546 7.545150 0.000000
      CHANNELS 3 Yrotation Xrotation Zrotation
      JOINT LeftShoulder
      {
       OFFSET 13.683099 0.000000 0.000000
       CHANNELS 3 Yrotation Xrotation Zrotation
       JOINT LeftElbow
       {
        OFFSET 26.359998 0.000000 0.000000
        CHANNELS 3 Yrotation Xrotation Zrotation
        JOINT LeftWrist
        {
         OFFSET 21.746691 0.000000 0.008601
         CHANNELS 3 Yrotation Xrotation Zrotation
         End Site
         {
          OFFSET 16.348058 0.000000 0.000000
         }
        }
       }
      }
     }
    }
   }
  }
 }
 JOINT RightHip
 {
  OFFSET -8.622479 -0.030774 -0.003140
  CHANNELS 3 Yrotation Xrotation Zrotation
  JOINT RightKnee
  {
   OFFSET 0.000000 -37.209160 -0.002630
   CHANNELS 3 Yrotation Xrotation Zrotation
   JOINT RightAnkle
   {
    OFFSET 0.000000 -37.343279 -0.058479
    CHANNELS 3 Yrotation Xrotation Zrotation
    JOINT RightToe
    {
     OFFSET 0.000000 -8.903465 15.088070
     CHANNELS 3 Yrotation Xrotation Zrotation
     End Site
     {
      OFFSET 0.000000 -1.471739 6.884388
     }
    }
   }
  }
 }
 JOINT LeftHip
 {
  OFFSET 8.622479 -0.030774 -0.003140
  CHANNELS 3 Yrotation Xrotation Zrotation
  JOINT LeftKnee
  {
   OFFSET 0.000000 -37.209160 -0.002630
   CHANNELS 3 Yrotation Xrotation Zrotation
   JOINT LeftAnkle
   {
    OFFSET 0.000000 -37.343279 -0.058479
    CHANNELS 3 Yrotation Xrotation Zrotation
    JOINT LeftToe
    {
     OFFSET 0.000000 -8.903465 15.088070
     CHANNELS 3 Yrotation Xrotation Zrotation
     End Site
     {
      OFFSET 0.000000 -1.471739 6.884388
     }
    }
   }
  }
 }
}
";

        [Test]
        public void GuessBoneMapping_mocap2()
        {
            var bvh = Bvh.Parse(bvh_mocap2);
            var detector = new BvhSkeletonEstimator();
            var skeleton = detector.Detect(bvh);

            Assert.AreEqual(0, skeleton.GetBoneIndex(HumanBodyBones.Hips));

            Assert.AreEqual("Hips", skeleton.GetBoneName(HumanBodyBones.Hips));

            Assert.AreEqual("Chest", skeleton.GetBoneName(HumanBodyBones.Spine));
            Assert.AreEqual("Chest2", skeleton.GetBoneName(HumanBodyBones.Chest));
            Assert.AreEqual("Chest4", skeleton.GetBoneName(HumanBodyBones.UpperChest));

            Assert.AreEqual("Neck", skeleton.GetBoneName(HumanBodyBones.Neck));
            Assert.AreEqual("Head", skeleton.GetBoneName(HumanBodyBones.Head));

            Assert.AreEqual("LeftCollar", skeleton.GetBoneName(HumanBodyBones.LeftShoulder));
            Assert.AreEqual("LeftShoulder", skeleton.GetBoneName(HumanBodyBones.LeftUpperArm));
            Assert.AreEqual("LeftElbow", skeleton.GetBoneName(HumanBodyBones.LeftLowerArm));
            Assert.AreEqual("LeftWrist", skeleton.GetBoneName(HumanBodyBones.LeftHand));

            Assert.AreEqual("RightCollar", skeleton.GetBoneName(HumanBodyBones.RightShoulder));
            Assert.AreEqual("RightShoulder", skeleton.GetBoneName(HumanBodyBones.RightUpperArm));
            Assert.AreEqual("RightElbow", skeleton.GetBoneName(HumanBodyBones.RightLowerArm));
            Assert.AreEqual("RightWrist", skeleton.GetBoneName(HumanBodyBones.RightHand));

            Assert.AreEqual("LeftHip", skeleton.GetBoneName(HumanBodyBones.LeftUpperLeg));
            Assert.AreEqual("LeftKnee", skeleton.GetBoneName(HumanBodyBones.LeftLowerLeg));
            Assert.AreEqual("LeftAnkle", skeleton.GetBoneName(HumanBodyBones.LeftFoot));
            Assert.AreEqual("LeftToe", skeleton.GetBoneName(HumanBodyBones.LeftToes));

            Assert.AreEqual("RightHip", skeleton.GetBoneName(HumanBodyBones.RightUpperLeg));
            Assert.AreEqual("RightKnee", skeleton.GetBoneName(HumanBodyBones.RightLowerLeg));
            Assert.AreEqual("RightAnkle", skeleton.GetBoneName(HumanBodyBones.RightFoot));
            Assert.AreEqual("RightToe", skeleton.GetBoneName(HumanBodyBones.RightToes));
        }
        #endregion
    }
}
