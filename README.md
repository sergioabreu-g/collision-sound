# Collision Sound for Unity </br> ![status-passing](https://img.shields.io/badge/build-passing-success) ![unity](https://img.shields.io/badge/unity-2017.4_onwards-informational)  ![fmod-studio](https://img.shields.io/badge/fmod-studio-informational)

Collision Sound aims to be an easy and powerful way of adding sound to
object collisions in [Unity](https://unity.com/). You can download the latest Unity package
[here](https://github.com/Sag-Dev/collision-sound/releases).

[DEMO VIDEO](https://www.youtube.com/watch?v=9sOHbRK5zb4)

# Getting started
Collision Sound requires the use of [Fmod Studio](https://www.fmod.com/studio)
to setup the material interactions and its parameters, so
[being familiar with it](https://www.fmod.com/learn) is strongly encouraged.

## Prerequisites
- [Fmod Studio](https://www.fmod.com/studio)
- [Unity](https://unity.com/) (from 2017.4 onwards)

## Installing
Before continuing, you should already have Unity and Fmod Studio installed.

- Download the [latest Unity package](https://github.com/Sag-Dev/collision-sound/releases)
- Import the package into your Unity project, it should now be ready
- Open the FMOD Studio project from your Unity project 'Assets/FmodProject' folder.
- If the project's version is different from your FMOD Studio installation, retarget the project
when opening, and it will work right away.

# How to use
This guide will be divided into two separate steps: the material events configuration
in FMOD Studio & the GameObjects setup in Unity.

## FMOD Studio
The FMOD Studio project structure can be summarized as follows:

*If you want to setup your custom FMOD Studio project, instead of using the one
already configured with the Unity package, read [this](https://www.fmod.com/resources/documentation-unity?version=2.0&page=user-guide.html#setting-up).*

`Remember that every time you change anything on the FMOD Studio project, you'll need to re-build it.`

### General
- All the materials and its events must be under the root folder `SoundMaterials`.
- Every collision event must be assigned to the `SoundMaterials` bank.
- Everything outside the `SoundMaterials` folder and bank will be ignored by Collision Sound when
importing the events from Unity.
- You can have any other folders/banks/events on the same FMOD Project, they won't affect Sound Collision
at all.

### Materials
- Every material is defined as a folder with at least its default event.
- The name of the folder will be the name of the material you'll then use in Unity.

### Default event
- Every material folder must have at least one event with the same name of the folder.
- The default event will be played if no other more specific event can be found.

### Interaction events
- To create events for specific collisions (`materialA` collides with `materialB`), create an event in the folder of one of the materials, and name it with the name of the other material (`:materialA/materialB`).
- Do not duplicate specific events by creating them under both material folders. If you do, one of them will be shadowed and it
can cause undesired behaviours.

## Unity
The only two things you have to handle in Unity are the `SoundCollider` components and the FMOD Studio Listener.
**Add an FMOD Studio Listener** to the object that will receive the sound (usually the main player of your game), then
**add a SoundCollider to any GameObject you want to make sounds on collision**.

#### Important!
`The SoundCollider script uses 'OnCollision' and 'OnTrigger' events, so in order for it to work, it must have a Rigidbody attached or be a trigger. This is not enforced or warned through code because in many cases it may be useful not to get those methods called. For example, you may have some objects that do not emit sound themselves when colliding, but are still treated as a SoundCollider, so other SoundColliders can have specific interactions with them.`

The following is a description of every attribute of the SoundCollider component:

![sound-collider-inspector](https://github.com/Sag-Dev/collision-sound/blob/master/_doc/sound-collider-inspector.png)

### General
- `Sound Material`: the name of this object's sound material (setup in the FMOD Studio project as explained above)
- `Require Another Sound Collider`: if set to true, sounds will only be played if the other gameobject
also has a SoundMaterial component.
- `Always Play Default Event`: if set to true, this SoundCollider will always play its default event
ignoring the events defined for specific material interactions.
- `Mute Other Default Events`: when this GameObject collides with another SoundCollider, it will force it NOT to play its default event. Specific interaction events will still be played.
- `Y Axis Is Forward 2D`: ONLY FOR 2D COLLIDERS. If set to true, Y axis will be treated
as forward/backward when positioning the sounds in 3D instead of up/down.

### Sound parameters
- `Volume`: volume the events of this SoundCollider will be played this. When playing specific interaction
events, the volume will be the average of both volumes.
- `Mute`: mute the SoundCollider. When two SoundColliders collide, if one of them is muted (or both) the event
will not be played.

### FMOD Studio parameters
- `Size Active`: whether to set the parameter *size* when playing collision events
- `Velocity Active`: whether to set the parameter *velocity* when playing collision events
- `Mass Active`: whether to set the parameter *mass* when playing collision events

## Parameters & automations
One of the more powerful tools of FMOD Studio is the parametrization & automation, and they're
integrated with Collision Sound. Keep reading to learn how to use them.

*For specific interaction events between two SoundColliders, the value of a parameter will be
the average of both their values (if both SoundColliders have the same parameter).*

### Built-in parameters
There are currently 2 built-in parameters synchronized with Unity, `size` and `velocity`.
If you setup parameters with those names in your events, they will be automatically set by
Unity on runtime.

So, for example, if you define a pitch decline as size gets bigger for one event, the pitch of
the event will be automatically adjusted on runtime depending on the current size of the object.

- `size`: magnitude of the object's collider size Vector3 (in world units).
- `velocity`: magnitude of the relative velocity Vector3 between the bodies of the collision
(if using triggers, it will the velocity of the colliding Rigidbody).
- `mass`: mass of the object's Rigidbody.

### Custom parameters
You can setup custom parameters for any of your events, and then set their values from code in Unity. To do so, just add any parameter to your collision events in FMOD Studio, then call call the method `setCustomParam(string parameter, float value)` of the SoundCollider that will be playing that event to set its value. You can also set all parameters at once by passing a `Dictionary<string, float>`, as well as get the parameters you've already set.

The custom parameters you set from code belong to the SoundCollider, not to its events. That means you can set any parameters for any SoundCollider, even if its events do not implement that parameter. That also means that when you call any of its `get` methods, **it will only return the parameters you've already setup from code**, not the ones you've configured in FMOD Studio for any of its events.

# Built with
- [Fmod Studio](https://www.fmod.com/studio)
- [Fmod Unity Integration](https://www.fmod.com/resources/documentation-unity)
- [Unity](https://unity.com/)

# Contributing
Any contribution will be more than welcome. If you find any sort of bug or
problem, you can open an issue so we can trace and fix it. If you want to fix
or implement a feature yourself, feel free to submit any pull request with your
changes and we'll merge it as soon as possible.

When opening an issue or submitting a pull request, please make sure of the
following:

- The issue/PR isn't a duplicate and hasn't been posted before.
- The title must clear and include only information relevant to the subject.
- The description of a bug must include the steps to reproduce it and a detailed
description of the problem.
- The description of a PR must include a general description followed by more detailed
explanation of the changes made.

# Authors
- [Sergio Abreu García](https://sag-dev.com)
- [Diego Martínez Simarro](https://github.com/dimart10)

# Acknowledgements
This is the final project for '*Sound in Videogames*' subject ([Universidad Complutense de Madrid](https://www.ucm.es/)), imparted by:
- [Jaime Sánchez Hernández](http://gpd.sip.ucm.es/jaime/)

# License
This project is under the
[Apache License 2.0](https://github.com/Sag-Dev/collision-sound/blob/master/LICENSE).
