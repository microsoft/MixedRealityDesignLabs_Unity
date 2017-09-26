using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iBounding {

    Bounds GetRelativeBounds(Transform targetTransform);
    Bounds GetRelativeCenter(Transform targetTransform);

    Bounds GetWorldBounds(Transform targetTransform);
    Bounds GetWorldCenter(Transform targetTransform);

}
