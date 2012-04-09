using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

class KinectGesturePlayer
{
    /// <summary>
    /// This is a Gesture module that will detect Gestures, users register callbacks
    /// DISCLAIMER: For best results users should be directly in front of the kinect. (Not at an angle)
    /// This might be a problem when we turn into two players, but for one player is decent.
    /// </summary>
    public delegate void dynamicCallBack(double change);
    public delegate void staticCallBack(bool exist);
    public delegate void listener();

    private Dictionary<listener, dynamicCallBack> dynamicCallBacks = new Dictionary<listener, dynamicCallBack>();
    private Dictionary<listener, staticCallBack> staticCallBacks = new Dictionary<listener, staticCallBack>();
    private bool handled = false;

    public DepthImagePoint lastHead, lastNeck, lastSpine, lastHipCenter;
    public DepthImagePoint lastLeftWrist, lastLeftHand, lastLeftElbow, lastLeftShoulder;
    public DepthImagePoint lastRightWrist, lastRightHand, lastRightElbow, lastRightShoulder;

    public DepthImagePoint currHead, currNeck, currSpine, currHipCenter;
    public DepthImagePoint currLeftWrist, currLeftHand, currLeftElbow, currLeftShoulder;
    public DepthImagePoint currRightWrist, currRightHand, currRightElbow, currRightShoulder;

    public void registerCallBack(listener listener, staticCallBack staticCallBack, dynamicCallBack dynamicCallback){
        if (staticCallBack != null)
        {
            staticCallBacks[listener] = staticCallBack;
        }
        if (dynamicCallback != null)
        {
            dynamicCallBacks[listener] = dynamicCallback;
        }    
    }

    private void callDynamicCallBack(listener listener, double change)
    {
        if (dynamicCallBacks.ContainsKey(listener))
        {
            dynamicCallBacks[listener](change);
        }
        else
        {
            // No one register this listener with a callback, so pass silently
        }
    }

    private void callStaticCallBack(listener listener, bool exists)
    {
        if (staticCallBacks.ContainsKey(listener))
        {
            staticCallBacks[listener](exists);
        }
        else
        {
            // No one register this listener with a callback, so pass silently
        }
    }

    public void skeletonReady(AllFramesReadyEventArgs e, Skeleton skeleton)
    {
        setJoints(e, skeleton);

        // Listeners
        /* The order does matter, it will look at them in order and short curcuit */
        handled = false;
        kinectGuideListener();
        handsAboveHeadListener();
        handsUppenListener();
        handsWidenListener();
        handSwingListener();
        fistsPumpListener();
    }

    public void setJoints(AllFramesReadyEventArgs e, Skeleton skeleton)
    {
        using (DepthImageFrame depth = e.OpenDepthImageFrame())
        {
            if (depth != null)
            {
                //Map a joint location to a point on the depth map
                lastHead = currHead;
                lastNeck = currNeck;
                lastSpine = currSpine;
                lastHipCenter = currHipCenter;

                lastLeftHand = currLeftHand;
                lastLeftWrist = currLeftWrist;
                lastLeftElbow = currLeftElbow;
                lastLeftShoulder = currLeftShoulder;

                lastRightHand = currRightHand;
                lastRightWrist = currRightWrist;
                lastRightElbow = currRightElbow;
                lastRightShoulder = currRightShoulder;

                currHead = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.Head].Position);
                currNeck = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.ShoulderCenter].Position);
                currSpine = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.Spine].Position);
                currHipCenter = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.HipCenter].Position);

                currLeftHand = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.HandLeft].Position);
                currLeftWrist = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.WristLeft].Position);
                currLeftElbow = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.ElbowLeft].Position);
                currLeftShoulder = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.ShoulderLeft].Position);

                currRightHand = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.HandRight].Position);
                currRightWrist = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.WristRight].Position);
                currRightElbow = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.ElbowRight].Position);
                currRightShoulder = depth.MapFromSkeletonPoint(skeleton.Joints[JointType.ShoulderRight].Position);
            }
        }
    }

    public static double GetBodySegmentAngle(DepthImagePoint j1, DepthImagePoint j2, DepthImagePoint j3)
    {
        return Vector3.angleBetween(new Vector3(j1.X, j1.Y, 0), new Vector3(j2.X, j2.Y, 0), new Vector3(j3.X, j3.Y, 0));
    }

    /* Guide gesture 
     * * http://support.xbox.com/en-US/kinect/body-tracking/body-controller
     * * If the user move their left arm at a ~45 degree angle about the vertical and have their right hand down
     * * this call a callback. We could use this for something like pause or something
     */
    private int guideTryCount = 0;
    public void kinectGuideListener()
    {
        if (handled)
        {
            callStaticCallBack(kinectGuideListener, false);
            guideTryCount = 0;
            return;
        }
        callDynamicCallBack(kinectGuideListener, currSpine.X);

        double leftAngle = GetBodySegmentAngle(currLeftHand, currLeftShoulder, currHipCenter);
        double rightAngle = GetBodySegmentAngle(currHipCenter, currRightShoulder, currRightHand);
        if ((rightAngle < 50 && rightAngle > 20) &&
            (leftAngle < 150 && leftAngle > 90))
        {
            if (guideTryCount >= 40)
            {
                callStaticCallBack(kinectGuideListener, true);
                handled = true;
            }
            else
            {
                guideTryCount++;
            }
        }
        else
        {
            callStaticCallBack(kinectGuideListener, false);
            if (guideTryCount > 0)
            {
                guideTryCount--;
            }
        }
    }

    /* Detects a right hand fist pump, that we might use for changing tempo */
    private bool fistLeftCharged = false;
    private bool fistLeftPumped = false;
    private bool fistRightCharged = false;
    private bool fistRightPumped = false;

    private int fistPumpTryCount = 0;
    public void fistsPumpListener()
    {
        if (handled)
        {
            callStaticCallBack(fistsPumpListener, false);
            fistLeftCharged = fistLeftPumped = fistLeftCharged = fistLeftPumped = false;
            fistPumpTryCount = 0;
            return;
        }

        double leftElbowAngle = GetBodySegmentAngle(currLeftShoulder, currLeftElbow, currLeftHand);
        bool fistLeftPumping = leftElbowAngle > 130;
        bool fistLeftCharging = leftElbowAngle < 90;

        double rightElbowAngle = GetBodySegmentAngle(currRightHand, currRightElbow, currRightShoulder);
        bool fistRightPumping = rightElbowAngle > 130;
        bool fistRightCharging = rightElbowAngle < 90;

        if ((currLeftHand.Y < currNeck.Y && (leftElbowAngle > 0 && leftElbowAngle < 180)) ||
                (currRightHand.Y < currNeck.Y && (rightElbowAngle > 0 && rightElbowAngle < 180)))
        {
            if (fistPumpTryCount >= 10)
            {
                if (currLeftHand.Y < currNeck.Y && (leftElbowAngle > 0 && leftElbowAngle < 180))
                {
                    fistPumpHelper(ref fistLeftCharged, ref fistLeftPumped, fistLeftCharging, fistLeftPumping, -1);
                }

                else if (currRightHand.Y < currNeck.Y && (rightElbowAngle > 0 && rightElbowAngle < 180))
                {
                    fistPumpHelper(ref fistRightCharged, ref fistRightPumped, fistRightCharging, fistRightPumping, 1);
                }
                handled = true;
            }
            else
            {
                fistPumpTryCount++;
            }
        }
        else
        {
            callStaticCallBack(fistsPumpListener, false);
            if (fistPumpTryCount >= 0)
            {
                fistPumpTryCount--;
            }
        }
    }


    private void fistPumpHelper(ref bool fistCharged, ref  bool fistPumped, bool fistCharging, bool fistPumping, int factor)
    {
        callStaticCallBack(fistsPumpListener, true);
        if (fistCharged && fistPumping)
        {
            callDynamicCallBack(fistsPumpListener, 10 * factor);
            fistCharged = false;
            fistPumped = true;
        }
        else if (fistPumped && fistCharging)
        {
            callDynamicCallBack(fistsPumpListener, 10 * factor);
            fistCharged = true;
            fistPumped = false;
        }

        fistPumped |= fistPumping;
        fistCharged |= fistCharging;
    }

    /* Detects a hand swing movement, that we might use for seeking */
    int seekTryCount = 0;
    public void handSwingListener()
    {
        if (handled)
        {
            callStaticCallBack(handSwingListener, false);
            seekTryCount = 0;
            return;
        }

        if (currRightHand.Y < currRightWrist.Y && currRightHand.Y > currHead.Y && currHead.Depth > currRightHand.Depth + 300 )
        {
            if (seekTryCount >= 10)
            {
                callStaticCallBack(handSwingListener, true);
                callDynamicCallBack(handSwingListener, currRightHand.X - lastRightHand.X);
                handled = true;
            }
            else
            {
                seekTryCount++;
            }
        }
        else
        {
            callStaticCallBack(handSwingListener, false);
            if (seekTryCount > 0)
            {
                seekTryCount--;
            }
        }
    }

    /* Detects hands above head, and tracks the movement of head difference,
     * * We could use this for pitch */
    private int handsAboveHeadTryCount = 0;
    public void handsAboveHeadListener()
    {
        if (handled)
        {
            callStaticCallBack(handsAboveHeadListener, false);
            handsAboveHeadTryCount = 0;
            return;
        }

        if (currHead.Y > currLeftHand.Y + 50 && currHead.Y > currRightHand.Y + 50)
        {
            if (handsAboveHeadTryCount >= 1)
            {
                callDynamicCallBack(handsAboveHeadListener, currHead.Y - lastHead.Y);
                callStaticCallBack(handsAboveHeadListener, true);
                handled = true;
            }
            else
            {
                handsAboveHeadTryCount++;
            }
        }
        else
        {
            if (handsAboveHeadTryCount > 0)
            {
                handsAboveHeadTryCount--;
            }
            callStaticCallBack(handsAboveHeadListener, false);
        }
    }

    /* Detects widening hands,
     * * We could use this for volume
     * * This is tracked by looking at both hands and seeing their position in relative to the hip and neck
     * */
    private double prevDist = 0;
    private bool wasTrackingHandsWiden = false;
    private int handsWidenTryCount = 0;
    public void handsWidenListener()
    {
        if (handled)
        {
            callStaticCallBack(handsWidenListener, false);
            wasTrackingHandsWiden = false;
            handsWidenTryCount = 0;
            return;
        }

        if ((currHipCenter.Y > currRightHand.Y && currRightHand.Y > currHead.Y) &&
            (currHipCenter.Y > currLeftHand.Y && currLeftHand.Y > currHead.Y))
        {
            if (handsWidenTryCount >= 5)
            {
                callStaticCallBack(handsWidenListener, true);
                double currDist = currRightHand.X - currLeftHand.X;
                if (wasTrackingHandsWiden)
                {
                    callDynamicCallBack(handsWidenListener, prevDist - currDist);
                }
                prevDist = currDist;
                wasTrackingHandsWiden = true;
                handled = true;
            }
            else
            {
                handsWidenTryCount++;
            }
        }
        else
        {
            callStaticCallBack(handsWidenListener, false);
            wasTrackingHandsWiden = false;
            if (handsWidenTryCount > 0)
            {
                handsWidenTryCount--;
            }
        }
    }


    /* Detects widening hands,
     * * We could use this for volume
     * * This is tracked by looking at both hands and seeing their position in relative to the hip and neck
     * */
    private double prevDist2 = 0;
    private bool wasTrackingHandsUppen = false;
    private int handsUppenTryCount = 0;
    public void handsUppenListener()
    {
        if (handled)
        {
            callStaticCallBack(handsUppenListener, false);
            wasTrackingHandsUppen = false;
            handsUppenTryCount = 0;
            return;
        }

        if ((currRightHand.X < currRightElbow.X) &&
            (currLeftHand.X > currLeftElbow.X) &&
            (currRightHand.Y < currLeftHand.Y))
        {
            if (handsUppenTryCount >= 5)
            {
                callStaticCallBack(handsUppenListener, true);
                double currDist = currRightHand.Y - currLeftHand.Y;
                if (wasTrackingHandsUppen)
                {
                    callDynamicCallBack(handsUppenListener, prevDist2 - currDist);
                }
                prevDist2 = currDist;
                wasTrackingHandsUppen = true;
                handled = true;
            }
            else
            {
                handsUppenTryCount++;
            }
        }
        else
        {
            callStaticCallBack(handsUppenListener, false);
            wasTrackingHandsUppen = false;
            if (handsUppenTryCount > 0)
            {
                handsUppenTryCount--;
            }
        }
    }

    private const int skeletonCount = 6;
    static private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
    public static Skeleton getFristSkeleton(AllFramesReadyEventArgs e)
    {
        using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
        {
            if (skeletonFrameData == null)
            {
                return null;
            }
            else
            {
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);
                //get the first tracked skeleton
                return (from s in allSkeletons
                        where s.TrackingState == SkeletonTrackingState.Tracked
                        select s).FirstOrDefault();
           }
        }
    }

    public static Skeleton[] getFirstTwoSkeletons(AllFramesReadyEventArgs e)
    {
        using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
        {
            if (skeletonFrameData == null)
            {
                return null;
            }
            else
            {
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);
                //get the first tracked skeleton
                Skeleton[] result = (from s in allSkeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).ToArray();

                if (result.Count() >= 2)
                {
                    if (result[0].Joints[JointType.Head].Position.X > result[1].Joints[JointType.Head].Position.X)
                    {
                        Skeleton temp = result[0];
                        result[0] = result[1];
                        result[1] = temp;
                    }
                }
                return result;
            }
        }
    }
}
