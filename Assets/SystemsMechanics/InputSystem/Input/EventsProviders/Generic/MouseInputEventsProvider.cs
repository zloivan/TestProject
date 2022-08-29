using UnityEngine;
using Zenject;
using UnityInput = UnityEngine.Input;

namespace Services.Input.EventsProviders.Generic
{
    public class MouseInputEventsProvider : BaseInputEventsProvider, IFixedTickable
    {
        private const int POINTER_ID = -1;
        
        private bool _isMouseButtonDown;
        private Vector3 _prevPosition;
        

        public void FixedTick()
        {
            if (!UnityInput.GetMouseButton(0))
            {
                if (_isMouseButtonDown)
                {
                    HandlePointerUp(UnityInput.mousePosition, POINTER_ID);
                    _isMouseButtonDown = false;
                }

                return;
            }

            var mousePosition = UnityInput.mousePosition;

            if (!_isMouseButtonDown)
            {
                _prevPosition = mousePosition;
                _isMouseButtonDown = true;
                HandlePointerDown(mousePosition, POINTER_ID);
            }
            else
            {
                HandlePointerMove(mousePosition, mousePosition - _prevPosition, POINTER_ID);
            }

            

            _prevPosition = mousePosition;
        }
    }
}