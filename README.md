# Collision Sound for Unity </br> ![status-on-development](https://img.shields.io/badge/On-DEVELOPMENT-ff69b4) ![unity](https://img.shields.io/badge/Unity-2019-informational) ![fmod-studio](https://img.shields.io/badge/FMOD-Studio-informational)

Collision Sound aims to be an easy and powerful way of adding sound to
object collisions in [Unity](https://unity.com/).

# Getting started
Collision Sound requires the use of [Fmod Studio](https://www.fmod.com/studio)
to setup the material interactions and its parameters, so
[being familiar with it](https://www.fmod.com/learn) is strongly encouraged.

## Prerequisites
- [Fmod Studio](https://www.fmod.com/studio)
- [Unity](https://unity.com/)

## Installing
Before continuing, you should already have Unity and Fmod Studio installed.

- Download the [latest Unity package](https://github.com/Sag-Dev/physical-sound/releases)
- Import the package into your Unity project, it should now be ready
- Open the FMOD Studio project under your Unity project 'Assets/Packages/CollisionSound/fmod_project'
- Keep reading to learn how to configure your collision events.

# How to use
This guide will be divided into two separate steps: the material events configuration
in FMOD Studio & the GameObjects setup in Unity.

`Every folder & event must be put under the master bank`

## FMOD Studio
The FMOD Studio project structure can be summarized as follows:

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

## Built-in parameters
There are currently 2 built-in parameters synchronized with Unity, `size` and `velocity`.
If you setup parameters with those names in your events, they will be automatically set by
Unity on runtime.

So, for example, if you define a pitch decline as size gets bigger for one event, the pitch of
the event will be automatically adjusted on runtime depending on the current size of the object.

- `size`: magnitude of the object's collider size Vector3 (in world units).
- `velocity`: magnitude of the relative velocity Vector3 between the bodies of the collision
(if using triggers, it will be always 0)


## Custom parameters
STILL NOT IMPLEMENTED

## Unity
The only thing you have to handle in Unity are the `SoundCollider` components.
**Add a SoundCollider to any GameObject you want to make sounds on collisions**, write down
a valid sound material name (setup in the FMOD Studio project as indicated in the previous
section) and it will work right away.

The following is a description of every attribute of the SoundCollider component, for more advanced usages:

![sound-collider-inspector](https://github.com/Sag-Dev/physical-sound/blob/master/_doc/sound-collider-inspector.png)

### General
- `Sound Material`: the name of this object's material
- `Require Another Sound Collider`: if set to true, sounds will only be played if the other gameobject
also has a SoundMaterial component.
- `Always Play Default Event`: if set to true, this SoundCollider will always play its default event
ignoring the events defined for specific material interactions.
- `Y Axis Is Forward 2D`: ONLY FOR 2D COLLIDERS. If set to true, Y axis will be treated
as forward/backward when positioning the sounds in 3D instead of up/down.

### Sound parameters
- `Volume`: volume the events of this SoundCollider will be played this. When playing specific interaction
events, the volume will be the average of both volumes.
- `Mute`: mute the SoundCollider. When two SoundColliders collide, if one of them is muted (or both) the event
will not be played.

### FMOD Studio parameters
- `Size Active`: whether to set parameter size when playing the events
- `Velocity Active`: whether to set parameter velocity when playing the events

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
[Apache License 2.0](https://github.com/Sag-Dev/physical-sound/blob/master/LICENSE).
