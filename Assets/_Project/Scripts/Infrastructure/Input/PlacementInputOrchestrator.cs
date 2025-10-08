using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectGame.Scripts.Infrastructure.Input
{
    public class PlacementInputOrchestrator
    {
        private const string ACTION_MAP_NAME = "Placement";
        private const string ACTION_MOVE = "Move";
        private const string ACTION_ROTATE_LEFT = "RotateLeft";
        private const string ACTION_ROTATE_RIGHT = "RotateRight";
        private const string ACTION_CONFIRM = "Confirm";
        private const string ACTION_CANCEL = "Cancel";
        private const string ACTION_POINT = "Point";
        private const string COMPOSITE_2D_VECTOR = "2DVector";
        private const string BIND_UP_W = "<Keyboard>/w";
        private const string BIND_DOWN_S = "<Keyboard>/s";
        private const string BIND_LEFT_A = "<Keyboard>/a";
        private const string BIND_RIGHT_D = "<Keyboard>/d";
        private const string BIND_UP_ARROW = "<Keyboard>/upArrow";
        private const string BIND_DOWN_ARROW = "<Keyboard>/downArrow";
        private const string BIND_LEFT_ARROW = "<Keyboard>/leftArrow";
        private const string BIND_RIGHT_ARROW = "<Keyboard>/rightArrow";
        private const string BIND_ROTATE_LEFT = "<Keyboard>/q";
        private const string BIND_ROTATE_RIGHT = "<Keyboard>/e";
        private const string BIND_CONFIRM_MOUSE = "<Mouse>/leftButton";
        private const string BIND_CONFIRM_ENTER = "<Keyboard>/enter";
        private const string BIND_CONFIRM_SPACE = "<Keyboard>/space";
        private const string BIND_CANCEL_MOUSE = "<Mouse>/rightButton";
        private const string BIND_CANCEL_ESC = "<Keyboard>/escape";
        private const string BIND_POINTER_POSITION = "<Pointer>/position";
        
        private const int ROTATION_STEP_DEGREES = 90;
        
        private readonly InputAction move;
        private readonly InputAction rotateLeft;
        private readonly InputAction rotateRight;
        private readonly InputAction confirm;
        private readonly InputAction cancel;
        private readonly InputAction point;

        public PlacementInputOrchestrator()
        {
            var map = new InputActionMap(ACTION_MAP_NAME);

            move = map.AddAction(ACTION_MOVE, binding: BIND_UP_W);
            move.AddCompositeBinding(COMPOSITE_2D_VECTOR).With("Up", BIND_UP_W)
                .With("Down", BIND_DOWN_S).With("Left", BIND_LEFT_A)
                .With("Right", BIND_RIGHT_D);
            move.AddCompositeBinding(COMPOSITE_2D_VECTOR).With("Up", BIND_UP_ARROW)
                .With("Down", BIND_DOWN_ARROW).With("Left", BIND_LEFT_ARROW)
                .With("Right", BIND_RIGHT_ARROW);
            
            rotateLeft = map.AddAction(ACTION_ROTATE_LEFT, binding: BIND_ROTATE_LEFT);
            rotateRight = map.AddAction(ACTION_ROTATE_RIGHT, binding: BIND_ROTATE_RIGHT);
            confirm = map.AddAction(ACTION_CONFIRM);
            confirm.AddBinding(BIND_CONFIRM_MOUSE);
            confirm.AddBinding(BIND_CONFIRM_ENTER);
            confirm.AddBinding(BIND_CONFIRM_SPACE);

            cancel = map.AddAction(ACTION_CANCEL);
            cancel.AddBinding(BIND_CANCEL_MOUSE);
            cancel.AddBinding(BIND_CANCEL_ESC);

            point = map.AddAction(ACTION_POINT, binding: BIND_POINTER_POSITION);

            map.Enable();

            move.performed += context =>
            {
                Vector2 value = context.ReadValue<Vector2>();
                PendingGridDelta = new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
            };
            move.canceled += _ => PendingGridDelta = Vector2Int.zero;
            rotateLeft.performed += _ => RotationDeltaDegrees -= ROTATION_STEP_DEGREES;
            rotateRight.performed += _ => RotationDeltaDegrees += ROTATION_STEP_DEGREES;
            confirm.performed += _ => ConfirmPressed = true;
            confirm.canceled += _ => ConfirmPressed = false;
            cancel.performed += _ => CancelPressed = true;
            cancel.canceled += _ => CancelPressed = false;
            point.performed += context => MouseScreenPosition = context.ReadValue<Vector2>();
        }
        
        public Vector2Int PendingGridDelta { get; private set; }
        public Vector2 MouseScreenPosition { get; private set; }
        public int RotationDeltaDegrees { get; private set; }
        public bool ConfirmPressed { get; private set; }
        public bool CancelPressed { get; private set; }

        public void ResetFrameDeltas()
        {
            RotationDeltaDegrees = 0;
        }
    }
}