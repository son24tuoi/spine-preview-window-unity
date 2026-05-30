Modified: May 30, 2026 09:34 PM

# Spine Preview Window - Add/Remove Event
---
⚠️ **Requires a project with [Spine Unity](https://esotericsoftware.com/spine-unity-download) already installed.**

---
[Download Package](https://github.com/son24tuoi/spine-preview-window-unity/releases)

[Source](https://github.com/son24tuoi/spine-preview-window-unity)

The Spine Preview Window is a visual panel within the Unity Engine that allows game developers and animators to quickly inspect, tweak, and manage Spine 2D files without having to open the dedicated Spine software or run the game directly.

>💡You can quickly add or remove events in animations, rather than relying on the Spine software as usual.

This tool supports the workflow, helping to quickly detect display errors and synchronize audio/effect events extremely effectively.

> **How to open Spine Preview:**
On the toolbar menu, select Tools → Spine Preview.

![SpinePreview-Demo.png](/Assets/SpinePreviewWindow/Images/SpinePreview-Demo.png)

# Benefits of using

- **Save time:** Quickly check character movements right in the Editor without having to play the animation.
- **Visual Event Management:** Attach audio/effect events with precision (milliseconds) based on the visual Timeline.
- **Quickly check for display errors:** Easily view bones or the mesh to detect clipping errors or distortion when the character moves.

# Main interface components

## 1. Visual Display Area

- **Preview Area:** Occupies a large space on the left, visually displaying objects and animation movements. Users can directly observe the character.
    - Scroll the mouse to zoom in or out on the object.
    - Drag the left mouse button to move the object.
- **Timeline Area:** Located at the bottom left, it displays the animation’s progress with a timeline (red) to help users see exactly where the animation is currently playing or where events are triggered (blue).
    - Click or drag the left mouse button to change the animation playback time.
    - Hover over the green marker to display event information.

## 2. Control Area

- **Skeleton Data Asset:** Select a Spine file.
- **Spine JSON Asset:** Automatically loaded based on the Skeleton Data Asset.
    - You can only edit the Event if this file is in JSON format.
- **Show Triangle Mesh:** Display the object’s triangle mesh.
- **Show Bones:** Displays the object’s bone system.
- **Fit Camera:** Adjust the camera to fit the object.
- **Skin: Select** the object's skin.
- **Setup Pose: Reset** the object to its default pose.
- **Animations**: Select the animation you want to test.
- **Loop:** The animation loops automatically.
- **Current Time: Displays** the current playback time; can be adjusted.
- **Speed: A slider** to adjust the animation playback speed (fast/slow).
- **Play:** Play the animation.
- **Pause: Pause** the animation.
- **Stop: Stops** the animation.
- **Event List: Displays** the list of events in the animation.
    - Click on an event to jump to the corresponding timestamp.
    - **Delete: Delete** an event.
    - **Create Event:** Create a new event.
        - **Name: Enter** the name of the new event.
        - **Time: Enter** the new event’s timestamp.
        - **Add Event:** Create a new event based on the entered details.
- **Save Events:** Save event changes if necessary.
    
>💡**Note:** Saving will create a new Spine JSON data file and a new Skeleton Data Asset file containing the content (the file you just edited will not be used).
>
> For example, if the original file is spineboy-unity.json, the new file will be spineboy-unity_202605241206.json. The added portion of the name is the file creation time.
>
> If you want to view the saved changes, select the Skeleton Data Asset file again.

# Extensions

- The package uses Newtonsoft.Json. Here are the installation instructions:
    1. On the menu bar, select **Window → Package Manager**.
    2. In the top-left corner of the Package Manager window, click the **+** sign and select **Add package by name…**
    3. Enter the name: **com.unity.nuget.newtonsoft-json and** click **Add**.