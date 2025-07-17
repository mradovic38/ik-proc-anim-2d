# 2D Inverse Kinematics Character with Procedural Limb Animation
A Unity project featuring a 2D character with procedural animation driven by inverse kinematics (IK). The animation is controlled programmatically without pre-made keyframes, enabling smooth, dynamic motion based on logic and math. 

## 🎨 Demo Preview
![ik 2d proc anim](https://github.com/user-attachments/assets/b4199e24-4aeb-4281-a6ad-c4b08c0232e0)


## 🎮 Features

* ✅ **Procedural Walking** using alternating foot placement with center of mass balance detection
* ✅ **Dynamic Foot IK** with smooth arc interpolation and step anticipation
* ✅ **Head Tracking**: character looks at the cursor with natural head tilt
* ✅ **Automatic Flipping**: direction-aware flipping of character and limbs
* ✅ **Modular Movement**: fully physics-based character controller
* ✅ **Gizmos Visualization** for step targets and debug purposes


## 🚀 How It Works

The walking algorithm works by checking if the character's **center of mass** is within the span of the two feet. If not, it triggers a **step** using a raycast to find the ground. The foot then **lerps in an arc** toward that point. The other foot waits its turn until the current step completes.

The **head rotation** system tracks the cursor only when idle and tilts naturally with the terrain using the spine's raycast normal.


## 📁 Scripts Overview

| Script Name            | Description                                                                 |
| ---------------------- | --------------------------------------------------------------------------- |
| `ProceduralWalking.cs` | Handles logic for walking by stepping each foot based on balance and timing |
| `FootPositioner.cs`    | Computes step paths for individual feet using raycasts and overshoot logic  |
| `PlayerMovement.cs`    | Simple Rigidbody2D-based movement controller for the character              |
| `HeadMovement.cs`      | Makes the head follow the mouse with spine rotation and vertical bobbing    |


## 📄 License

This project is licensed under the MIT License.
Feel free to use or modify for your own projects!

