using UnityEngine;
using extOSC;

public class SkeletonController : MonoBehaviour
{
    public OSCReceiver receiver; // Message receiver
    public GameObject skelly;
    public Transform[] skeletonPoints;  // Landmarks positions (33 points)
    public Animator animator;       // For bone rig
    public float scaleFactor = 1f;

    // MediaPipe landmark to bone mapping
    private int[] landmarkToBoneMapping = new int[]
    {
        0,  // Head (nose)
        2,  // Eye
        5,
        11, // Left Shoulder
        12, // Right Shoulder
        13, // Left Elbow
        14, // Right Elbow
        15, // Left Wrist
        16, // Right Wrist
        23, // Left Hip
        24, // Right Hip
        25, // Left Knee
        26, // Right Knee
        29,
        30,
        31,
        32
    };

    void Start()
    {
        skeletonPoints = new Transform[33]; // MediaPipe tem 33 landmarks

        for (int i = 0; i < skeletonPoints.Length; i++)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);   // Desenha esferas (debug)
            //GameObject point = new GameObject("point " + i);
            point.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            skeletonPoints[i] = point.transform;

            // recebe info das coordenadas de cada ponto
            receiver.Bind("/pose/landmark_" + i, OnReceivePoseLandmark);
        }
    }

    // Update landmark positions
    private void OnReceivePoseLandmark(OSCMessage message)
    {
        if (message.ToString().Contains("/pose/landmark_"))
        {
            int index = int.Parse(message.Address.Split('_')[1]);

            float x = message.Values[0].FloatValue;
            float y = message.Values[1].FloatValue;
            float z = message.Values[2].FloatValue;

            // Fit Unity's world space
            Vector3 newPosition = new Vector3(-x, -y, 0);

            // Interpolate between positions
            Vector3 interpolatedPosition = Vector3.Lerp(skeletonPoints[index].position, newPosition, 0.2f);

            if (index >= 0 && index < skeletonPoints.Length)
            {
                skeletonPoints[index].position = interpolatedPosition;
            }

            ApplyToRig(index, interpolatedPosition);
        }
    }

    private void ApplyToRig(int index, Vector3 landmarkPosition)
    {
        Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        spine.transform.position = Vector3.Lerp(skeletonPoints[23].position, skeletonPoints[24].position, 0.5f);
        Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        hips.transform.position = Vector3.Lerp(skeletonPoints[23].position, skeletonPoints[24].position, 0.5f);

        // Mapping the bones
        if (index == 0)
        {
            Transform headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            if (headBone != null)
            {
                headBone.position = landmarkPosition;
                //RotateBoneTowards(headBone, landmarkPosition);
            }
        }
        if (index == 2 || index == 5)
        {
            HumanBodyBones eyeBone = (index == 2) ? HumanBodyBones.LeftEye : HumanBodyBones.RightEye;
            Transform eye = animator.GetBoneTransform(eyeBone);
            if (eye != null)
            {
                eye.position = landmarkPosition;
                //RotateBoneTowards(eye, landmarkPosition);
            }
        }
        Transform shoulderL = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        Transform shoulderR = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        shoulderL.transform.position = Vector3.Lerp(skeletonPoints[11].position, skeletonPoints[12].position, 0.5f);
        shoulderR.transform.position = Vector3.Lerp(skeletonPoints[11].position, skeletonPoints[12].position, 0.5f);

        if (index == 11 || index == 12)
        {
            HumanBodyBones shoulderBone = (index == 11) ? HumanBodyBones.LeftUpperArm : HumanBodyBones.RightUpperArm;
            Transform shoulder = animator.GetBoneTransform(shoulderBone);
            if (shoulder != null)
            {
                RotateBoneTowards(shoulder, landmarkPosition);
            }
        }
        if (index == 13 || index == 14)
        {
            HumanBodyBones elbowBone = (index == 13) ? HumanBodyBones.LeftLowerArm : HumanBodyBones.RightLowerArm;
            Transform elbow = animator.GetBoneTransform(elbowBone);
            if (elbow != null)
            {
                RotateBoneTowards(elbow, landmarkPosition);
            }
        }
        if (index == 15 || index == 16)
        {
            HumanBodyBones wristBone = (index == 15) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand;
            Transform wrist = animator.GetBoneTransform(wristBone);
            if (wrist != null)
            {
                RotateBoneTowards(wrist, landmarkPosition);
            }
        }

        if (index == 23 || index == 24)
        {
            HumanBodyBones hipBone = (index == 23) ? HumanBodyBones.LeftUpperLeg : HumanBodyBones.RightUpperLeg;
            Transform hip = animator.GetBoneTransform(hipBone);
            if (hip != null)
            {
                RotateBoneTowards(hip, landmarkPosition);
            }
        }
        if (index == 25 || index == 26)
        {
            HumanBodyBones kneeBone = (index == 25) ? HumanBodyBones.LeftLowerLeg : HumanBodyBones.RightLowerLeg;
            Transform knee = animator.GetBoneTransform(kneeBone);
            if (knee != null)
            {
                RotateBoneTowards(knee, landmarkPosition);
            }
        }
        if (index == 29 || index == 30)
        {
            HumanBodyBones footBone = (index == 29) ? HumanBodyBones.LeftFoot : HumanBodyBones.RightFoot;
            Transform foot = animator.GetBoneTransform(footBone);
            if (foot != null)
            {
                RotateBoneTowards(foot, landmarkPosition);
            }
        }
        if (index == 31 || index == 32)
        {
            HumanBodyBones toesBone = (index == 31) ? HumanBodyBones.LeftToes : HumanBodyBones.RightToes;
            Transform toes = animator.GetBoneTransform(toesBone);
            if (toes != null)
            {
                RotateBoneTowards(toes, landmarkPosition);
            }
        }
    }

    // Rotate a bone towards the landmark position
    private void RotateBoneTowards(Transform bone, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - bone.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        //bone.position = targetPosition;
        bone.rotation = Quaternion.Slerp(bone.rotation, targetRotation, 0.5f); // Smoothly rotate
    }
}