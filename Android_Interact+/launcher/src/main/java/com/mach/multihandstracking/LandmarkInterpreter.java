package com.mach.multihandstracking;

import com.google.mediapipe.formats.proto.LandmarkProto;
import com.unity3d.player.UnityPlayer;

import java.util.List;

/**
 * Handles all the interpretation of the landmark coordinates.
 */
public class LandmarkInterpreter {

    //Flag for the left- and right hand support
    public static boolean hand_right = false;

    /**
     * Interprets the landmark 2D-coordinates and runs different
     * functions accordingly of the interpretation.
     *
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     */
    public static void gestureInterpreter(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return;
        }

        // Recognizes the "OK" gesture and moves the selection pointer
        // and the pointer in the accuracy game accordingly to
        // the positions of the landmark 2D-coordinates.
        if (handGestureRecognition(multiHandLandmarks).equals("ok")) {
            UnityPlayer.UnitySendMessage("SystemManager", "EvaluateFingerPos", thumbOnePosition(multiHandLandmarks));
            UnityPlayer.UnitySendMessage("SystemManager", "MoveOkSelectionPointer", thumbOneGesturePosition(multiHandLandmarks));
            return;
        }

        // Recognizes the "Five" gesture with a bent thumb, which is the gesture for confirmation
        // for the "Five" approach.
        // Calls the evaluation functions for collisions with objects in the unity component / in
        // the virtual environment.
        if (handGestureRecognition(multiHandLandmarks).equals("four")) {
            UnityPlayer.UnitySendMessage("SystemManager", "CheckCollisionGestureOk", "");
            UnityPlayer.UnitySendMessage("SystemManager", "EvaluateOkSelection", "");

            UnityPlayer.UnitySendMessage("SystemManager", "CheckCollisionGestureFive", "");
            UnityPlayer.UnitySendMessage("SystemManager", "EvaluateFiveSelection", "");
            return;
        }

        // Recognizes the "Five" gesture, which is the gesture for confirmation for the "OK" approach.
        // It either confirms an action for the "OK" approach or moves the selection pointer and the
        // pointer in the accuracy game accordingly to the positions of the landmark 2D-coordinates
        // for the "Five" approach.
        if (handGestureRecognition(multiHandLandmarks).equals("five")) {
            UnityPlayer.UnitySendMessage("SystemManager", "CheckCollisionGestureOk", "");
            UnityPlayer.UnitySendMessage("SystemManager", "EvaluateOkSelection", "");

            UnityPlayer.UnitySendMessage("SystemManager", "EvaluatPalmPos", palmPosition((multiHandLandmarks)));
            UnityPlayer.UnitySendMessage("SystemManager", "MoveFiveSelectionPointer", palmGesturePosition((multiHandLandmarks)));
            return;
        }
    }

    /**
     * Translate a value from one range of values and scales it up/down to a different range of values.
     * Used for translation of the normalized 2D-coordinates accordingly
     * to the size of the field of the accuracy game
     *
     * @param value    Value that is about to be translated
     * @param leftMin  Minimum of the first range of values
     * @param leftMax  Maximum of the first range of values
     * @param rightMin Minimum of the second range of values
     * @param rightMax Maximum of the second range of values
     * @return Translated Value to the second range of values
     */
    public static Float translate(float value, float leftMin, float leftMax, float rightMin, float rightMax) {
        //Figure out how 'wide' each range is
        float leftSpan = leftMax - leftMin;
        float rightSpan = rightMax - rightMin;

        //Convert the left range into 0-1 range (float)
        float valueScaled = (value - leftMin) / leftSpan;
        return rightMin + (valueScaled * rightSpan);
    }

    /**
     * Changes the flag for the left- and right hand support
     */
    public static void handRight() {
        hand_right = !hand_right;
    }

    /**
     * Translates the normalized coordinates of the thumb and the index finger to the field of the
     * accuracy game in the virtual environment.
     * Used for moving the pointer of the accuracy game of the "OK" approach.
     *
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     * @return Translated coordinates of the thumb and the index finger.
     */
    public static String thumbOnePosition(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return "";
        }

        LandmarkProto.NormalizedLandmark thumb = multiHandLandmarks.get(0).getLandmark(4);
        LandmarkProto.NormalizedLandmark first = multiHandLandmarks.get(0).getLandmark(8);

        float leftMinX = 0.3f; // Left Border of X of Detection
        float leftMinY = 0.3f; // Upper Border of Y of Detection

        float leftMaxX = 0.7f; // Right Border of X of Detection
        float leftMaxY = 0.7f; // Lower Border of Y of Detection


        //Should not be changed
        float rightMinX = -3.5f; // Left Border of X of Canvas
        float rightMinY = -1.5f; // Upper Border of Y of Canvas

        float rightMaxX = 3.5f; // Right Border of X of Canvas
        float rightMaxY = 1.5f; // Lower Border of Y of Canvas

        return translate(thumb.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(thumb.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY) + "|"
                + translate(first.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(first.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY);
    }

    /**
     * Translates the normalized coordinates of the thumb and the index finger to the
     * field of view of the user in the virtual environment.
     * Used for moving the selection pointer of the "OK" approach.
     *
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     * @return Translated coordinates of the thumb and the index finger.
     */
    public static String thumbOneGesturePosition(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return "";
        }

        LandmarkProto.NormalizedLandmark thumb = multiHandLandmarks.get(0).getLandmark(4);
        LandmarkProto.NormalizedLandmark first = multiHandLandmarks.get(0).getLandmark(8);

        float leftMinX = 0.3f; // Left Border of X of Detection
        float leftMinY = 0.3f; // Upper Border of Y of Detection

        float leftMaxX = 0.7f; // Right Border of X of Detection
        float leftMaxY = 0.7f; // Lower Border of Y of Detection


        //Should not be changed
        float rightMinX = -70f; // Left Border of X of Canvas
        float rightMinY = -70f; // Upper Border of Y of Canvas

        float rightMaxX = 70f; // Right Border of X of Canvas
        float rightMaxY = 70f; // Lower Border of Y of Canvas

        return translate(thumb.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(thumb.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY) + "|"
                + translate(first.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(first.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY);
    }

    /**
     * Translates the normalized coordinates of two landmarks that are in the middle of
     * the hand to the field of the accuracy game in the virtual environment.
     * Used for moving the pointer of the accuracy game of the "Five" approach.
     *
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     * @return Translated coordinates of two landmarks that are in the middle of the hand.
     */
    public static String palmPosition(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return "";
        }

        LandmarkProto.NormalizedLandmark leftMid = multiHandLandmarks.get(0).getLandmark(9);
        LandmarkProto.NormalizedLandmark rightMid = multiHandLandmarks.get(0).getLandmark(13);

        float leftMinX = 0.3f; // Left Border of X of Detection
        float leftMinY = 0.3f; // Upper Border of Y of Detection

        float leftMaxX = 0.7f; // Right Border of X of Detection
        float leftMaxY = 0.7f; // Lower Border of Y of Detection


        //Should not be changed
        float rightMinX = -3.5f; // Left Border of X of Canvas
        float rightMinY = -1.5f; // Upper Border of Y of Canvas

        float rightMaxX = 3.5f; // Right Border of X of Canvas
        float rightMaxY = 1.5f; // Lower Border of Y of Canvas


        return translate(leftMid.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(leftMid.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY) + "|"
                + translate(rightMid.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(rightMid.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY);
    }

    /**
     * Translates the normalized coordinates of two landmarks that are in the middle of
     * the hand to the field of view of the user.
     * Used for moving the selection pointer of the "Five" approach.
     *
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     * @return Translated coordinates of two landmarks that are in the middle of the hand.
     */
    public static String palmGesturePosition(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return "";
        }

        LandmarkProto.NormalizedLandmark leftMid = multiHandLandmarks.get(0).getLandmark(9);
        LandmarkProto.NormalizedLandmark rightMid = multiHandLandmarks.get(0).getLandmark(13);

        float leftMinX = 0.3f; // Left Border of X of Detection
        float leftMinY = 0.3f; // Upper Border of Y of Detection

        float leftMaxX = 0.7f; // Right Border of X of Detection
        float leftMaxY = 0.7f; // Lower Border of Y of Detection


        //Should not be changed
        float rightMinX = -70f; // Left Border of X of Canvas
        float rightMinY = -70f; // Upper Border of Y of Canvas

        float rightMaxX = 70; // Right Border of X of Canvas
        float rightMaxY = 70f; // Lower Border of Y of Canvas


        return translate(leftMid.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(leftMid.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY) + "|"
                + translate(rightMid.getX(), leftMinX, leftMaxX, rightMinX, rightMaxX) + "|"
                + -translate(rightMid.getY(), leftMinY, leftMaxY, rightMinY, rightMaxY);
    }

    /**
     * Checks if the hand is recognized or not.
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     * @return True or false, if the hand is recognized or not.
     */
    public static Boolean handRecognition(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        return !multiHandLandmarks.isEmpty();
    }

    /**
     * Evaluates the normalized landmark 2D-coordinates and returns a string, which tells the user
     * what gesture is recognized currently
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     * @return The recognized gesture.
     */
    public static String handGestureRecognition(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return "";
        }

        LandmarkProto.NormalizedLandmarkList landmarkList = multiHandLandmarks.get(0);

        //finger states
        boolean thumbIsOpen = false;
        boolean firstFingerIsOpen = false;
        boolean secondFingerIsOpen = false;
        boolean thirdFingerIsOpen = false;
        boolean fourthFingerIsOpen = false;

        //Thumb open?
        float pseudoFixKeyPoint = landmarkList.getLandmark(2).getX();
        if (hand_right && (landmarkList.getLandmark(3).getX() < pseudoFixKeyPoint && landmarkList.getLandmark(4).getX() < pseudoFixKeyPoint)) {
            thumbIsOpen = true;
        } else if (!hand_right && (landmarkList.getLandmark(3).getX() < pseudoFixKeyPoint && landmarkList.getLandmark(4).getX() < pseudoFixKeyPoint)) {
            thumbIsOpen = true;
        }
        //First finger open?
        pseudoFixKeyPoint = landmarkList.getLandmark(6).getY();
        if (landmarkList.getLandmark(7).getY() < pseudoFixKeyPoint && landmarkList.getLandmark(8).getY() < pseudoFixKeyPoint) {
            firstFingerIsOpen = true;
        }

        //Second finger open?
        pseudoFixKeyPoint = landmarkList.getLandmark(10).getY();
        if (landmarkList.getLandmark(11).getY() < pseudoFixKeyPoint && landmarkList.getLandmark(12).getY() < pseudoFixKeyPoint) {
            secondFingerIsOpen = true;
        }

        //Third finger open?
        pseudoFixKeyPoint = landmarkList.getLandmark(14).getY();
        if (landmarkList.getLandmark(15).getY() < pseudoFixKeyPoint && landmarkList.getLandmark(16).getY() < pseudoFixKeyPoint) {
            thirdFingerIsOpen = true;
        }

        //Forth finger open?
        pseudoFixKeyPoint = landmarkList.getLandmark(18).getY();
        if (landmarkList.getLandmark(19).getY() < pseudoFixKeyPoint && landmarkList.getLandmark(20).getY() < pseudoFixKeyPoint) {
            fourthFingerIsOpen = true;
        }

        //Hand gesture recognition
        if (thumbIsOpen && firstFingerIsOpen && secondFingerIsOpen && thirdFingerIsOpen && fourthFingerIsOpen) {
            return "five";
        } else if (!thumbIsOpen && firstFingerIsOpen && secondFingerIsOpen && thirdFingerIsOpen && fourthFingerIsOpen) {
            return "four";
        } else if (thumbIsOpen && firstFingerIsOpen && secondFingerIsOpen && !thirdFingerIsOpen && !fourthFingerIsOpen) {
            return "three";
        } else if (thumbIsOpen && firstFingerIsOpen && !secondFingerIsOpen && !thirdFingerIsOpen && !fourthFingerIsOpen) {
            return "two";
        } else if (!thumbIsOpen && firstFingerIsOpen && !secondFingerIsOpen && !thirdFingerIsOpen && !fourthFingerIsOpen) {
            return "one";
        } else if (!thumbIsOpen && !firstFingerIsOpen && !secondFingerIsOpen && !thirdFingerIsOpen && !fourthFingerIsOpen) {
            return "fist";
        } else if (!thumbIsOpen && firstFingerIsOpen && secondFingerIsOpen && !thirdFingerIsOpen && !fourthFingerIsOpen) {
            return "peace";
        } else if (thumbIsOpen && !firstFingerIsOpen && !secondFingerIsOpen && !thirdFingerIsOpen && !fourthFingerIsOpen) {
            return "thumb";
        } else if (!thumbIsOpen && firstFingerIsOpen && !secondFingerIsOpen && !thirdFingerIsOpen && fourthFingerIsOpen) {
            return "ROCK'N'ROLL!";
        } else if (thumbIsOpen && firstFingerIsOpen && !secondFingerIsOpen && !thirdFingerIsOpen && fourthFingerIsOpen) {
            return "SPIDERMAN!";
        } else if (!firstFingerIsOpen && secondFingerIsOpen && thirdFingerIsOpen && fourthFingerIsOpen && isThumbNearFirstFinger(landmarkList.getLandmark(4), landmarkList.getLandmark(8), 0.3f)) {
            return "ok";
        } else {
            return "Finger States: " + thumbIsOpen + " " + firstFingerIsOpen + " " + secondFingerIsOpen + " " + thirdFingerIsOpen + " " + fourthFingerIsOpen;
        }
    }

    /**
     * Calculates the euclidean distance between two points.
     *
     * @param a_X X-coordinate of the first point
     * @param a_Y Y-coordinate of the first point
     * @param b_X X-coordinate of the second point
     * @param b_Y Y-coordinate of the second point
     * @return The distance between the points.
     */
    private static float get_Euclidean_DistanceAB(float a_X, float a_Y, float b_X, float b_Y) {
        float dist = (float) (Math.pow(a_X - b_X, 2) + Math.pow(a_Y - b_Y, 2));
        return (float) Math.sqrt(dist);
    }

    /**
     * Checks if the distance between two points is less than the given distance.
     * @param point1 First point
     * @param point2 Second points
     * @param pDistance Given distance
     * @return If the distance between the points is less than the given distance.
     */
    private static boolean isThumbNearFirstFinger(LandmarkProto.NormalizedLandmark point1, LandmarkProto.NormalizedLandmark point2, float pDistance) {
        float distance = get_Euclidean_DistanceAB(point1.getX(), point1.getY(), point2.getX(), point2.getY());
        return distance < pDistance;
    }

    /**
     * For debug purposes. Return every coordinate of the landmarks as string.
     * @param multiHandLandmarks Normalized landmark 2D-coordinates
     * @return Every normalized landmark coordinate as string.
     */
    public static String getMultiHandLandmarksDebugString(List<LandmarkProto.NormalizedLandmarkList> multiHandLandmarks) {
        if (multiHandLandmarks.isEmpty()) {
            return "No hand landmarks";
        }
        String multiHandLandmarksStr = "Number of hands detected: " + multiHandLandmarks.size() + "\n";
        int handIndex = 0;
        for (LandmarkProto.NormalizedLandmarkList landmarks : multiHandLandmarks) {
            multiHandLandmarksStr +=
                    "\t#Hand landmarks for hand[" + handIndex + "]: " + landmarks.getLandmarkCount() + "\n";
            int landmarkIndex = 0;
            for (LandmarkProto.NormalizedLandmark landmark : landmarks.getLandmarkList()) {
                multiHandLandmarksStr +=
                        "\t\tLandmark ["
                                + landmarkIndex
                                + "]: ("
                                + landmark.getX()
                                + ", "
                                + landmark.getY()
                                + ", "
                                + landmark.getZ()
                                + ")\n";
                ++landmarkIndex;
            }
            ++handIndex;
        }
        return multiHandLandmarksStr;
    }

}
