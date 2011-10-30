using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VCSkinner;

namespace MCSkin3D
{
	//-----------------------------------------------------------------------------
	// A general purpose 6DoF (six degrees of freedom) quaternion based camera.
	//
	// This camera class supports 4 different behaviors:
	// first person mode, spectator mode, flight mode, and orbit mode.
	//
	// First person mode only allows 5DOF (x axis movement, y axis movement, z axis
	// movement, yaw, and pitch) and movement is always parallel to the world x-z
	// (ground) plane.
	//
	// Spectator mode is similar to first person mode only movement is along the
	// direction the camera is pointing.
	// 
	// Flight mode supports 6DoF. This is the camera class' default behavior.
	//
	// Orbit mode rotates the camera around a target position. This mode can be
	// used to simulate a third person camera. Orbit mode supports 2 modes of
	// operation: orbiting about the target's Y axis, and free orbiting. The former
	// mode only allows pitch and yaw. All yaw changes are relative to the target's
	// local Y axis. This means that camera yaw changes won't be affected by any
	// rolling. The latter mode allows the camera to freely orbit the target. The
	// camera is free to pitch, yaw, and roll. All yaw changes are relative to the
	// camera's orientation (in space orbiting the target).
	//
	// This camera class allows the camera to be moved in 2 ways: using fixed
	// step world units, and using a supplied velocity and acceleration. The former
	// simply moves the camera by the specified amount. To move the camera in this
	// way call one of the move() methods. The other way to move the camera
	// calculates the camera's displacement based on the supplied velocity,
	// acceleration, and elapsed time. To move the camera in this way call the
	// updatePosition() method.
	//-----------------------------------------------------------------------------

	public class NewCamera
	{
		public enum CameraBehavior
		{
			CAMERA_BEHAVIOR_FIRST_PERSON,
			CAMERA_BEHAVIOR_SPECTATOR,
			CAMERA_BEHAVIOR_FLIGHT,
			CAMERA_BEHAVIOR_ORBIT
		};

		public const float DEFAULT_ROTATION_SPEED = 0.3f;
		public const float DEFAULT_FOVX = 90.0f;
		public const float DEFAULT_ZNEAR = 0.1f;
		public const float DEFAULT_ZFAR = 1000.0f;

		public const float DEFAULT_ORBIT_MIN_ZOOM = DEFAULT_ZNEAR + 1.0f;
		public const float DEFAULT_ORBIT_MAX_ZOOM = DEFAULT_ZFAR * 0.5f;

		public const float DEFAULT_ORBIT_OFFSET_DISTANCE = DEFAULT_ORBIT_MIN_ZOOM +
			(DEFAULT_ORBIT_MAX_ZOOM - DEFAULT_ORBIT_MIN_ZOOM) * 0.25f;

		public static Vector3 WORLD_XAXIS = new Vector3(1.0f, 0.0f, 0.0f);
		public static Vector3 WORLD_YAXIS = new Vector3(0.0f, 1.0f, 0.0f);
		public static Vector3 WORLD_ZAXIS = new Vector3(0.0f, 0.0f, 1.0f);

		private CameraBehavior m_behavior;
		private bool m_preferTargetYAxisOrbiting;
		private float m_accumPitchDegrees;
		private float m_savedAccumPitchDegrees;
		private float m_rotationSpeed;
		private float m_fovx;
		private float m_aspectRatio;
		private float m_znear;
		private float m_zfar;
		private float m_orbitMinZoom;
		private float m_orbitMaxZoom;
		private float m_orbitOffsetDistance;
		private float m_firstPersonYOffset;
		private Vector3 m_eye = Vector3.Empty;
		private Vector3 m_savedEye = Vector3.Empty;
		private Vector3 m_target = Vector3.Empty;
		private Vector3 m_targetYAxis = Vector3.Empty;
		private Vector3 m_xAxis = Vector3.Empty;
		private Vector3 m_yAxis = Vector3.Empty;
		private Vector3 m_zAxis = Vector3.Empty;
		private Vector3 m_viewDir = Vector3.Empty;
		private Vector3 m_acceleration = Vector3.Empty;
		private Vector3 m_currentVelocity = Vector3.Empty;
		private Vector3 m_velocity = Vector3.Empty;
		private Quaternion m_orientation = new Quaternion();
		private Quaternion m_savedOrientation = new Quaternion();
		private Matrix4 m_viewMatrix = new Matrix4();
		private Matrix4 m_projMatrix = new Matrix4();
		private Matrix4 m_viewProjMatrix = new Matrix4();

		public NewCamera()
		{
			m_behavior = CameraBehavior.CAMERA_BEHAVIOR_FLIGHT;
			m_preferTargetYAxisOrbiting = true;
    
			m_accumPitchDegrees = 0.0f;
			m_savedAccumPitchDegrees = 0.0f;
    
			m_rotationSpeed = DEFAULT_ROTATION_SPEED;
			m_fovx = DEFAULT_FOVX;
			m_aspectRatio = 0.0f;
			m_znear = DEFAULT_ZNEAR;
			m_zfar = DEFAULT_ZFAR;   
    
			m_orbitMinZoom = DEFAULT_ORBIT_MIN_ZOOM;
			m_orbitMaxZoom = 99999;
			m_orbitOffsetDistance = DEFAULT_ORBIT_OFFSET_DISTANCE;
        
			m_eye.Set(0.0f, 0.0f, 0.0f);
			m_savedEye.Set(0.0f, 0.0f, 0.0f);
			m_target.Set(0.0f, 0.0f, 0.0f);
			m_xAxis.Set(1.0f, 0.0f, 0.0f);
			m_yAxis.Set(0.0f, 1.0f, 0.0f);
			m_targetYAxis.Set(0.0f, 1.0f, 0.0f);
			m_zAxis.Set(0.0f, 0.0f, 1.0f);
			m_viewDir.Set(0.0f, 0.0f, -1.0f);
        
			m_acceleration.Set(0.0f, 0.0f, 0.0f);
			m_currentVelocity.Set(0.0f, 0.0f, 0.0f);
			m_velocity.Set(0.0f, 0.0f, 0.0f);

			m_orientation.identity();
			m_savedOrientation.identity();

			m_viewMatrix.identity();
			m_projMatrix.identity();
			m_viewProjMatrix.identity();
		}

		~NewCamera()
		{
		}

		public void lookAt(Vector3 target)
		{
			lookAt(m_eye, target, m_yAxis);
		}

		public void lookAt(Vector3 eye, Vector3 target, Vector3 up)
		{
			m_eye = eye;
			m_target = target;

			m_zAxis = eye - target;
			m_zAxis.Normalize();

			m_viewDir = -m_zAxis;

			m_xAxis = up.CrossProduct (m_zAxis);
			m_xAxis.Normalize();

			m_yAxis = m_zAxis.CrossProduct (m_xAxis);
			m_yAxis.Normalize();
    
			m_viewMatrix[0,0] = m_xAxis.x;
			m_viewMatrix[1,0] = m_xAxis.y;
			m_viewMatrix[2,0] = m_xAxis.z;
			m_viewMatrix[3,0] = -m_xAxis.DotProduct (eye);

			m_viewMatrix[0,1] = m_yAxis.x;
			m_viewMatrix[1,1] = m_yAxis.y;
			m_viewMatrix[2,1] = m_yAxis.z;
			m_viewMatrix[3,1] = -m_yAxis.DotProduct(eye);

			m_viewMatrix[0,2] = m_zAxis.x;
			m_viewMatrix[1,2] = m_zAxis.y;
			m_viewMatrix[2,2] = m_zAxis.z;    
			m_viewMatrix[3,2] = -m_zAxis.DotProduct(eye);

			// Extract the pitch angle from the view matrix.
			m_accumPitchDegrees = VCMath.radiansToDegrees((float)Math.Asin(m_viewMatrix[1,2]));
    
			m_orientation.fromMatrix(m_viewMatrix);
			updateViewMatrix();
		}

		public void move(float dx, float dy, float dz)
		{
			// Moves the camera by dx world units to the left or right; dy
			// world units upwards or downwards; and dz world units forwards
			// or backwards.

			if (m_behavior == CameraBehavior.CAMERA_BEHAVIOR_ORBIT)
			{
				// Orbiting camera is always positioned relative to the
				// target position. See updateViewMatrix().
				return;
			}

			Vector3 eye = m_eye;
			Vector3 forwards;

			if (m_behavior == CameraBehavior.CAMERA_BEHAVIOR_FIRST_PERSON)
			{
				// Calculate the forwards direction. Can't just use the camera's local
				// z axis as doing so will cause the camera to move more slowly as the
				// camera's view approaches 90 degrees straight up and down.

				forwards = WORLD_YAXIS.CrossProduct(m_xAxis);
				forwards.Normalize();
			}
			else
			{
				forwards = m_viewDir;
			}

			eye += m_xAxis * dx;
			eye += WORLD_YAXIS * dy;
			eye += forwards * dz;

			setPosition(eye);
		}

		public void move(Vector3 direction, Vector3 amount)
		{
			// Moves the camera by the specified amount of world units in the specified
			// direction in world space.

			if (m_behavior == CameraBehavior.CAMERA_BEHAVIOR_ORBIT)
			{
				// Orbiting camera is always positioned relative to the
				// target position. See updateViewMatrix().
				return;
			}

			m_eye.x += direction.x * amount.x;
			m_eye.y += direction.y * amount.y;
			m_eye.z += direction.z * amount.z;

			updateViewMatrix();
		}

		public void perspective(float fovx, float aspect, float znear, float zfar)
		{
			//ruct a projection matrix based on the horizontal field of view
			// 'fovx' rather than the more traditional vertical field of view 'fovy'.

			float e = 1.0f / (float)Math.Tan(VCMath.degreesToRadians(fovx) / 2.0f);
			float aspectInv = 1.0f / aspect;
			float fovy = 2.0f * (float)Math.Atan(aspectInv / e);
			float xScale = 1.0f / (float)Math.Tan(0.5f * fovy);
			float yScale = xScale / aspectInv;

			m_projMatrix[0,0] = xScale;
			m_projMatrix[0,1] = 0.0f;
			m_projMatrix[0,2] = 0.0f;
			m_projMatrix[0,3] = 0.0f;

			m_projMatrix[1,0] = 0.0f;
			m_projMatrix[1,1] = yScale;
			m_projMatrix[1,2] = 0.0f;
			m_projMatrix[1,3] = 0.0f;

			m_projMatrix[2,0] = 0.0f;
			m_projMatrix[2,1] = 0.0f;
			m_projMatrix[2,2] = (zfar + znear) / (znear - zfar);
			m_projMatrix[2,3] = -1.0f;

			m_projMatrix[3,0] = 0.0f;
			m_projMatrix[3,1] = 0.0f;
			m_projMatrix[3,2] = (2.0f * zfar * znear) / (znear - zfar);
			m_projMatrix[3,3] = 0.0f;

			m_viewProjMatrix = m_viewMatrix * m_projMatrix;
    
			m_fovx = fovx;
			m_aspectRatio = aspect;
			m_znear = znear;
			m_zfar = zfar;
		}

		public void rotate(float headingDegrees, float pitchDegrees, float rollDegrees)
		{
			// Rotates the camera based on its current behavior.
			// Note that not all behaviors support rolling.

			pitchDegrees = -pitchDegrees;
			headingDegrees = -headingDegrees;
			rollDegrees = -rollDegrees;

			switch (m_behavior)
			{
			default:
				break;

			case CameraBehavior.CAMERA_BEHAVIOR_FIRST_PERSON:
			case CameraBehavior.CAMERA_BEHAVIOR_SPECTATOR:
				rotateFirstPerson(headingDegrees, pitchDegrees);
				break;

			case CameraBehavior.CAMERA_BEHAVIOR_FLIGHT:
				rotateFlight(headingDegrees, pitchDegrees, rollDegrees);
				break;

			case CameraBehavior.CAMERA_BEHAVIOR_ORBIT:
				rotateOrbit(headingDegrees, pitchDegrees, rollDegrees);
				break;
			}

			updateViewMatrix();
		}

		public void rotateSmoothly(float headingDegrees, float pitchDegrees, float rollDegrees)
		{
			// This method applies a scaling factor to the rotation angles prior to
			// using these rotation angles to rotate the camera. This method is usually
			// called when the camera is being rotated using an input device (such as a
			// mouse or a joystick). 

			headingDegrees *= m_rotationSpeed;
			pitchDegrees *= m_rotationSpeed;
			rollDegrees *= m_rotationSpeed;

			rotate(headingDegrees, pitchDegrees, rollDegrees);
		}

		public void undoRoll()
		{
			// Undo any camera rolling by leveling the camera. When the camera is
			// orbiting this method will cause the camera to become level with the
			// orbit target.

			if (m_behavior == CameraBehavior.CAMERA_BEHAVIOR_ORBIT)
				lookAt(m_eye, m_target, m_targetYAxis);
			else
				lookAt(m_eye, m_eye + m_viewDir, WORLD_YAXIS);
		}

		public void updatePosition(Vector3 direction, float elapsedTimeSec)
		{
			// Moves the camera using Newton's second law of motion. Unit mass is
			// assumed here to somewhat simplify the calculations. The direction vector
			// is in the range [-1,1].

			if (m_currentVelocity.LengthSq() != 0.0f)
			{
				// Only move the camera if the velocity vector is not of zero length.
				// Doing this guards against the camera slowly creeping around due to
				// floating point rounding errors.

				Vector3 displacement = (m_currentVelocity * elapsedTimeSec) +
					(0.5f * m_acceleration * elapsedTimeSec * elapsedTimeSec);

				// Floating point rounding errors will slowly accumulate and cause the
				// camera to move along each axis. To prevent any unintended movement
				// the displacement vector is clamped to zero for each direction that
				// the camera isn't moving in. Note that the updateVelocity() method
				// will slowly decelerate the camera's velocity back to a stationary
				// state when the camera is no longer moving along that direction. To
				// account for this the camera's current velocity is also checked.

				if (direction.x == 0.0f && VCMath.closeEnough(m_currentVelocity.x, 0.0f))
					displacement.x = 0.0f;

				if (direction.y == 0.0f && VCMath.closeEnough(m_currentVelocity.y, 0.0f))
					displacement.y = 0.0f;

				if (direction.z == 0.0f && VCMath.closeEnough(m_currentVelocity.z, 0.0f))
					displacement.z = 0.0f;

				move(displacement.x, displacement.y, displacement.z);
			}

			// Continuously update the camera's velocity vector even if the camera
			// hasn't moved during this call. When the camera is no longer being moved
			// the camera is decelerating back to its stationary state.

			updateVelocity(direction, elapsedTimeSec);
		}

		public void zoom(float zoom, float minZoom, float maxZoom)
		{
			if (m_behavior == CameraBehavior.CAMERA_BEHAVIOR_ORBIT)
			{
				// Moves the camera closer to or further away from the orbit
				// target. The zoom amounts are in world units.

				m_orbitMaxZoom = maxZoom;
				m_orbitMinZoom = minZoom;

				Vector3 offset = m_eye - m_target;

				m_orbitOffsetDistance = offset.Length();
				offset.Normalize();
				m_orbitOffsetDistance += zoom;
				m_orbitOffsetDistance = Math.Min(Math.Max(m_orbitOffsetDistance, minZoom), maxZoom);

				offset *= m_orbitOffsetDistance;
				m_eye = offset + m_target;
        
				updateViewMatrix();
			}
			else
			{
				// For the other behaviors zoom is interpreted as changing the
				// horizontal field of view. The zoom amounts refer to the horizontal
				// field of view in degrees.

				zoom = Math.Min(Math.Max(zoom, minZoom), maxZoom);
				perspective(zoom, m_aspectRatio, m_znear, m_zfar);
			}
		}

		// Getter methods.

		public Vector3 getAcceleration()
		{ return m_acceleration; }

		public CameraBehavior getBehavior()
		{ return m_behavior; }

		public Vector3 getCurrentVelocity()
		{ return m_currentVelocity; }

		public Vector3 getPosition()
		{ return m_eye; }

		public float getOrbitMinZoom()
		{ return m_orbitMinZoom; }

		public float getOrbitMaxZoom()
		{ return m_orbitMaxZoom; }

		public float getOrbitOffsetDistance()
		{ return m_orbitOffsetDistance; }

		public Quaternion getOrientation()
		{ return m_orientation; }

		public float getRotationSpeed()
		{ return m_rotationSpeed; }

		public Matrix4 getProjectionMatrix()
		{ return m_projMatrix; }

		public Vector3 getVelocity()
		{ return m_velocity; }

		public Vector3 getViewDirection()
		{ return m_viewDir; }

		public Matrix4 getViewMatrix()
		{ return m_viewMatrix; }

		public Matrix4 getViewProjectionMatrix()
		{ return m_viewProjMatrix; }

		public Vector3 getXAxis()
		{ return m_xAxis; }

		public Vector3 getYAxis()
		{ return m_yAxis; }

		public Vector3 getZAxis()
		{ return m_zAxis; }

		public Vector3 getEye()
		{ return m_eye; }

		public Vector3 getTarget()
		{ return m_target; }

		public bool preferTargetYAxisOrbiting()
		{ return m_preferTargetYAxisOrbiting; }
    
		// Setter methods.

		public void setAcceleration(Vector3 acceleration)
		{
			m_acceleration = acceleration;
		}

		public void setBehavior(CameraBehavior newBehavior)
		{
			// Switch to a new camera mode (i.e., behavior).
			// This method is complicated by the fact that it tries to save the current
			// behavior's state prior to making the switch to the new camera behavior.
			// Doing this allows seamless switching between camera behaviors.

			CameraBehavior prevBehavior = m_behavior;

			if (prevBehavior == newBehavior)
				return;

			m_behavior = newBehavior;

			switch (newBehavior)
			{
			case CameraBehavior.CAMERA_BEHAVIOR_FIRST_PERSON:
				switch (prevBehavior)
				{
				default:
					break;

				case CameraBehavior.CAMERA_BEHAVIOR_FLIGHT:
					m_eye.y = m_firstPersonYOffset;
					updateViewMatrix();
					break;

				case CameraBehavior.CAMERA_BEHAVIOR_SPECTATOR:
					m_eye.y = m_firstPersonYOffset;
					updateViewMatrix();
					break;

				case CameraBehavior.CAMERA_BEHAVIOR_ORBIT:
					m_eye.x = m_savedEye.x;
					m_eye.z = m_savedEye.z;
					m_eye.y = m_firstPersonYOffset;
					m_orientation = m_savedOrientation;
					m_accumPitchDegrees = m_savedAccumPitchDegrees;
					updateViewMatrix();
					break;
				}

				undoRoll();
				break;

			case CameraBehavior.CAMERA_BEHAVIOR_SPECTATOR:
				switch (prevBehavior)
				{
				default:
					break;

				case CameraBehavior.CAMERA_BEHAVIOR_FLIGHT:
					updateViewMatrix();
					break;

				case CameraBehavior.CAMERA_BEHAVIOR_ORBIT:
					m_eye = m_savedEye;
					m_orientation = m_savedOrientation;
					m_accumPitchDegrees = m_savedAccumPitchDegrees;
					updateViewMatrix();
					break;
				}

				undoRoll();
				break;
    
			case CameraBehavior.CAMERA_BEHAVIOR_FLIGHT:
				if (prevBehavior == CameraBehavior.CAMERA_BEHAVIOR_ORBIT)
				{
					m_eye = m_savedEye;
					m_orientation = m_savedOrientation;
					m_accumPitchDegrees = m_savedAccumPitchDegrees;
					updateViewMatrix();
				}
				else
				{
					m_savedEye = m_eye;
					updateViewMatrix();
				}
				break;
    
			case CameraBehavior.CAMERA_BEHAVIOR_ORBIT:
				if (prevBehavior == CameraBehavior.CAMERA_BEHAVIOR_FIRST_PERSON)
					m_firstPersonYOffset = m_eye.y;

				m_savedEye = m_eye;
				m_savedOrientation = m_orientation;
				m_savedAccumPitchDegrees = m_accumPitchDegrees;
        
				m_targetYAxis = m_yAxis;

				Vector3 newEye = m_eye + m_zAxis * m_orbitOffsetDistance;
				Vector3 newTarget = m_eye;
        
				lookAt(newEye, newTarget, m_targetYAxis);
				break;
			}
		}

		public void setCurrentVelocity(Vector3 currentVelocity)
		{
			m_currentVelocity = currentVelocity;
		}

		public void setCurrentVelocity(float x, float y, float z)
		{
			m_currentVelocity.Set(x, y, z);
		}

		public void setOrbitMaxZoom(float orbitMaxZoom)
		{
			m_orbitMaxZoom = orbitMaxZoom;
		}

		public void setOrbitMinZoom(float orbitMinZoom)
		{
			m_orbitMinZoom = orbitMinZoom;
		}

		public void setOrbitOffsetDistance(float orbitOffsetDistance)
		{
			m_orbitOffsetDistance = orbitOffsetDistance;
		}

		public void setOrientation(Quaternion newOrientation)
		{
			Matrix4 m = newOrientation.toMatrix4();

			// Store the pitch for this new orientation.
			// First person and spectator behaviors limit pitching to
			// 90 degrees straight up and down.

			m_accumPitchDegrees = VCMath.radiansToDegrees((float)Math.Asin(m[1,2]));

			// First person and spectator behaviors don't allow rolling.
			// Negate any rolling that might be encoded in the new orientation.

			m_orientation = newOrientation;

			if (m_behavior == CameraBehavior.CAMERA_BEHAVIOR_FIRST_PERSON || m_behavior == CameraBehavior.CAMERA_BEHAVIOR_SPECTATOR)
				lookAt(m_eye, m_eye + m_viewDir, WORLD_YAXIS);

			updateViewMatrix();
		}

		public void setPosition(Vector3 newEye)
		{
			m_eye = newEye;
			updateViewMatrix();
		}

		public void setPreferTargetYAxisOrbiting(bool preferTargetYAxisOrbiting)
		{
			// Determines the behavior of Y axis rotations when the camera is
			// orbiting a target. When preferTargetYAxisOrbiting is true all
			// Y axis rotations are about the orbit target's local Y axis.
			// When preferTargetYAxisOrbiting is false then the camera's
			// local Y axis is used instead.

			m_preferTargetYAxisOrbiting = preferTargetYAxisOrbiting;

			if (m_preferTargetYAxisOrbiting)
				undoRoll();
		}

		public void setRotationSpeed(float rotationSpeed)
		{
			// This is just an arbitrary value used to scale rotations
			// when rotateSmoothly() is called.

			m_rotationSpeed = rotationSpeed;
		}

		public void setVelocity(Vector3 velocity)
		{
			m_velocity = velocity;
		}

		public void setVelocity(float x, float y, float z)
		{
			m_velocity.Set(x, y, z);
		}

		public void rotateFirstPerson(float headingDegrees, float pitchDegrees)
		{
			// Implements the rotation logic for the first person style and
			// spectator style camera behaviors. Roll is ignored.

			m_accumPitchDegrees += pitchDegrees;

			if (m_accumPitchDegrees > 90.0f)
			{
				pitchDegrees = 90.0f - (m_accumPitchDegrees - pitchDegrees);
				m_accumPitchDegrees = 90.0f;
			}

			if (m_accumPitchDegrees < -90.0f)
			{
				pitchDegrees = -90.0f - (m_accumPitchDegrees - pitchDegrees);
				m_accumPitchDegrees = -90.0f;
			}
   
			Quaternion rot = new Quaternion();

			// Rotate camera about the world y axis.
			// Note the order the quaternions are multiplied. That is important!
			if (headingDegrees != 0.0f)
			{
				rot.fromAxisAngle(WORLD_YAXIS, headingDegrees);
				m_orientation = rot * m_orientation;
			}

			// Rotate camera about its local x axis.
			// Note the order the quaternions are multiplied. That is important!
			if (pitchDegrees != 0.0f)
			{
				rot.fromAxisAngle(WORLD_XAXIS, pitchDegrees);
				m_orientation = m_orientation * rot;
			}
		}

		public void rotateFlight(float headingDegrees, float pitchDegrees, float rollDegrees)
		{
			// Implements the rotation logic for the flight style camera behavior.

			m_accumPitchDegrees += pitchDegrees;

			if (m_accumPitchDegrees > 360.0f)
				m_accumPitchDegrees -= 360.0f;

			if (m_accumPitchDegrees < -360.0f)
				m_accumPitchDegrees += 360.0f;
   
			Quaternion rot = new Quaternion();

			rot.fromHeadPitchRoll(headingDegrees, pitchDegrees, rollDegrees);
			m_orientation *= rot;
		}

		public void rotateOrbit(float headingDegrees, float pitchDegrees, float rollDegrees)
		{
			// Implements the rotation logic for the orbit style camera behavior.
			// Roll is ignored for target Y axis orbiting.
			//
			// Briefly here's how this orbit camera implementation works. Switching to
			// the orbit camera behavior via the setBehavior() method will set the
			// camera's orientation to match the orbit target's orientation. Calls to
			// rotateOrbit() will rotate this orientation. To turn this into a third
			// person style view the updateViewMatrix() method will move the camera
			// position back 'm_orbitOffsetDistance' world units along the camera's
			// local z axis from the orbit target's world position.
    
			Quaternion rot = new Quaternion();

			if (m_preferTargetYAxisOrbiting)
			{
				if (headingDegrees != 0.0f)
				{
					rot.fromAxisAngle(m_targetYAxis, headingDegrees);
					m_orientation = rot * m_orientation;
				}

				if (pitchDegrees != 0.0f)
				{
					rot.fromAxisAngle(WORLD_XAXIS, pitchDegrees);
					m_orientation = m_orientation * rot;
				}
			}
			else
			{
				rot.fromHeadPitchRoll(headingDegrees, pitchDegrees, rollDegrees);
				m_orientation *= rot;
			}
		}

		public void updateVelocity(Vector3 direction, float elapsedTimeSec)
		{
			// Updates the camera's velocity based on the supplied movement direction
			// and the elapsed time (since this method was last called). The movement
			// direction is in the range [-1,1].

			if (direction.x != 0.0f)
			{
				// Camera is moving along the x axis.
				// Linearly accelerate up to the camera's max speed.

				m_currentVelocity.x += direction.x * m_acceleration.x * elapsedTimeSec;

				if (m_currentVelocity.x > m_velocity.x)
					m_currentVelocity.x = m_velocity.x;
				else if (m_currentVelocity.x < -m_velocity.x)
					m_currentVelocity.x = -m_velocity.x;
			}
			else
			{
				// Camera is no longer moving along the x axis.
				// Linearly decelerate back to stationary state.

				if (m_currentVelocity.x > 0.0f)
				{
					if ((m_currentVelocity.x -= m_acceleration.x * elapsedTimeSec) < 0.0f)
						m_currentVelocity.x = 0.0f;
				}
				else
				{
					if ((m_currentVelocity.x += m_acceleration.x * elapsedTimeSec) > 0.0f)
						m_currentVelocity.x = 0.0f;
				}
			}

			if (direction.y != 0.0f)
			{
				// Camera is moving along the y axis.
				// Linearly accelerate up to the camera's max speed.

				m_currentVelocity.y += direction.y * m_acceleration.y * elapsedTimeSec;
        
				if (m_currentVelocity.y > m_velocity.y)
					m_currentVelocity.y = m_velocity.y;
				else if (m_currentVelocity.y < -m_velocity.y)
					m_currentVelocity.y = -m_velocity.y;
			}
			else
			{
				// Camera is no longer moving along the y axis.
				// Linearly decelerate back to stationary state.

				if (m_currentVelocity.y > 0.0f)
				{
					if ((m_currentVelocity.y -= m_acceleration.y * elapsedTimeSec) < 0.0f)
						m_currentVelocity.y = 0.0f;
				}
				else
				{
					if ((m_currentVelocity.y += m_acceleration.y * elapsedTimeSec) > 0.0f)
						m_currentVelocity.y = 0.0f;
				}
			}

			if (direction.z != 0.0f)
			{
				// Camera is moving along the z axis.
				// Linearly accelerate up to the camera's max speed.

				m_currentVelocity.z += direction.z * m_acceleration.z * elapsedTimeSec;

				if (m_currentVelocity.z > m_velocity.z)
					m_currentVelocity.z = m_velocity.z;
				else if (m_currentVelocity.z < -m_velocity.z)
					m_currentVelocity.z = -m_velocity.z;
			}
			else
			{
				// Camera is no longer moving along the z axis.
				// Linearly decelerate back to stationary state.

				if (m_currentVelocity.z > 0.0f)
				{
					if ((m_currentVelocity.z -= m_acceleration.z * elapsedTimeSec) < 0.0f)
						m_currentVelocity.z = 0.0f;
				}
				else
				{
					if ((m_currentVelocity.z += m_acceleration.z * elapsedTimeSec) > 0.0f)
						m_currentVelocity.z = 0.0f;
				}
			}
		}

		public void updateViewMatrix()
		{
			// Reconstruct the view matrix.

			m_viewMatrix = m_orientation.toMatrix4();

			m_xAxis.Set(m_viewMatrix[0,0], m_viewMatrix[1,0], m_viewMatrix[2,0]);
			m_yAxis.Set(m_viewMatrix[0,1], m_viewMatrix[1,1], m_viewMatrix[2,1]);
			m_zAxis.Set(m_viewMatrix[0,2], m_viewMatrix[1,2], m_viewMatrix[2,2]);
			m_viewDir = -m_zAxis;

			if (m_behavior == CameraBehavior.CAMERA_BEHAVIOR_ORBIT)
			{
				// Calculate the new camera position based on the current
				// orientation. The camera must always maintain the same
				// distance from the target. Use the current offset vector
				// to determine the correct distance from the target.

				m_eye = m_target + m_zAxis * m_orbitOffsetDistance;
			}

			m_viewMatrix[3,0] = -m_xAxis.DotProduct (m_eye);
			m_viewMatrix[3,1] = -m_yAxis.DotProduct (m_eye);
			m_viewMatrix[3,2] = -m_zAxis.DotProduct (m_eye);
		}
	};
}
