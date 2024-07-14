# Frustum Culling Solution for Unity

## Overview
The **Frustum Culling Solution** is Unity asset that enhances performance by culling both dynamic and static objects without relying on Unity’s built-in system. This solution does not require scene baking, allowing for the dynamic addition and spawning of objects. It leverages Unity's Burst Jobs to optimize calculations, ensuring efficient and rapid culling operations. It will work best on top down views. Its not using ray casting just checks if objects are in plane frustrum.

## Features
- **Dynamic and Static Object Culling**: Supports both dynamic and static objects.
- **No Scene Baking Required**: Works without the need for scene baking.
- **Dynamic Object Spawning**: Allows for the addition of objects dynamically.
- **Optimized with Burst Jobs**: Uses Burst Jobs to speed up calculations, enhancing performance.
- **Customizable Bounds**: Offers multiple sources for bounds calculation (Renderers, Colliders, or Custom).

## Installation
1. Import the Frustum Culling Solution package into your Unity project.
2. Add the `FrustumCullingController` to your scene.
3. Attach the `FrustumCullingObject` component to any GameObject you want to be culled.

## Components

### FrustumCullingObject
The `FrustumCullingObject` component handles the culling of individual objects.

#### Properties
- **objectType**: Specifies whether the object is static or dynamic.
- **boundsSource**: Determines the source of the bounds (Renderers, Colliders, or Custom).
- **renderers**: An array of renderers associated with the object.
- **particleSystems**: An array of particle systems associated with the object.
- **behaviours**: An array of behaviours (such as Animators) that should be enabled or disabled based on visibility.
- **colliders**: An array of colliders associated with the object.

#### Methods
- **Initialise**: Initializes the `FrustumCullingObject` with the specified type and bounds source.
- **SetVisibility**: Sets the visibility of the object (called by `FrustumCullingController`).
- **AddBehaviours**: Adds additional behaviours to be controlled by visibility changes.
- **AddRenderers**: Adds additional renderers to be controlled by visibility changes.
- **AddColliders**: Adds additional colliders to be controlled by visibility changes.
- **FetchComponents**: Fetches associated components (renderers, colliders, particle systems, and behaviours).

#### Events
- **onBecomeVisible**: Called when object becomes visible
- **onBecomeInvisible**: Called when object becomes invisible
- **onCalculateBounds**: Called when bounds are calculated. You can modify bounds result in this event.

### FrustumCullingController
The `FrustumCullingController` manages the culling process for all `FrustumCullingObject` instances.

#### Properties
- **cameraTarget**: Specifies which camera to use for culling calculations (Main, Current, or Selected).
- **targetCamera**: The selected camera to be used when `cameraTarget` is set to `Selected`.
- **bufferSize**: The buffer size for internal collections.
- **refreshRate**: The rate at which the culling calculations are refreshed.

#### Methods
- **ChangeTargetCamera**: Changes the target camera for the culling system.
- **Add**: Adds a `FrustumCullingItem` to the controller (called internally).
- **Remove**: Removes a `FrustumCullingItem` from the controller (called internally).
- **UpdateItem**: Updates a `FrustumCullingItem` in the controller (called internally).

## Usage

### Setting Up the Scene
1. **Add `FrustumCullingController`**:
   - Create an empty GameObject and attach the `FrustumCullingController` component.
   - Configure the `cameraTarget` and `targetCamera` properties as needed.

2. **Add `FrustumCullingObject`**:
   - Attach the `FrustumCullingObject` component to any GameObject you want to be culled.
   - Configure the `objectType`, `boundsSource`, and other properties as needed.

### Dynamic Object Management
- **Initialize New Objects**:
  ```csharp
  var frustumCullingObject = gameObject.AddComponent<FrustumCullingObject>();
  frustumCullingObject.Initialise(FrustumCullingObjectType.Dynamic, FrustumCullingBoundsSource.Renderers);

  Bounds customBounds = new Bounds(Vector3.zero, new Vector3(1, 1, 1));
  frustumCullingObject.Initialise(FrustumCullingObjectType.Static, FrustumCullingBoundsSource.Custom, customBounds);
  ```
- **Add Components Dynamically**:
  ```charp
  frustumCullingObject.AddRenderers(new List<Renderer> { myRenderer });
  frustumCullingObject.AddColliders(new List<Collider> { myCollider });
  frustumCullingObject.AddBehaviours(new List<Behaviour> { myAnimator });
  ```
- **Changing Target Camera**
  ```charp
  frustumCullingController.ChangeTargetCamera(FrustumCullingSystemCameraTarget.Selected, myCustomCamera);
  ```

## Performance Tips
- **Adjust Buffer Size:** Ensure the buffer size is appropriate for the number of objects being culled.
- **Set Refresh Rate:** Adjust the refresh rate based on the performance needs of your application.
- **Optimize Bounds Calculation:** Use appropriate bounds sources to minimize the overhead of bounds calculation.
