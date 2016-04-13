using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{

    // Кожа героя состоит из лучей, который воспринимают коллизии
	private const float SkinWidth = .02f;
	private const int TotalHorizontalRays = 8;
	private const int TotalVerticalRays = 4;

    /// <summary>
    ///  Тангенс 75 градусов
    /// </summary>
	private static readonly float SlopeLimitTangant = Mathf.Tan(75f * Mathf.Deg2Rad);

	public LayerMask PlatformMask;
	public ControllerParameters2D DefaultParameters;

	public ControllerState2D State { get; private set; }

	public Vector2 Velocity { get {return _velocity;} }

    /// <summary>
    /// Управление движением и просчета физики игрока игрока
    /// </summary>
	public bool HandleCollisions { get; set; }

    /// <summary>
    /// Обьект на котором персонаж стоит
    /// </summary>
	public GameObject StandingOn { get; private set; }

	public Vector3 PlatformVelocity { get; private set; }

    public ControllerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }


    public bool CanJump
	{
		get
		{
			if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpAnywhere)
				return _jumpIn <= 0;

			if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpOnGround)
				return State.IsGrounded;

			return false;
		}
	}

	private Vector2 _velocity;
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private ControllerParameters2D _overrideParameters;
	private float _jumpIn;
	private GameObject _lastStandingOn;

	private Vector3
		_activeGlobalPlatformPoint,
		_activeLocalPlatformPoint;

	private Vector3
		_raycastTopLeft,
		_raycastBottomRight,
		_raycastBottomLeft;

	private float
		_verticalDistanceBetweenRays,
		_horizontalDistanceBetweenRays;

    public void Awake()
	{
		HandleCollisions = true;
		State = new ControllerState2D();

		_transform = transform;
		_localScale = transform.localScale;
		_boxCollider = GetComponent<BoxCollider2D>();

        // Рассчитываем расстояние между "лучами" кожи.
        // по вертикали - 4 луча
        // по горизонтали - 8 лучей

        // длина коллайдера в зависимости от масштаба
		var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth);
        // что бы разбить коллайдер на 3 части нужно его длину разделить на 2
        // так как крайние лучи на границах коллайдера то отнимаем и их длину
        // |______________|
        // ||     ||     ||

        #region TODO
        // ТОDO: Проверить или 
        // var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (TotalVerticalRays * SkinWidth);
        #endregion

        _horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

        // высота коллайдера в зависимости от масштаба (аналогично ширине)
		var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
	}

	public void AddForce(Vector2 force)
	{
		_velocity = force;
	}

	public void SetForce(Vector2 force)
	{
		_velocity += force;
	}

	public void SetHorizontalForce(float x)
	{
		_velocity.x = x;
	}

	public void SetVerticalForce(float y)
	{
		_velocity.y = y;
	}

	public void Jump()
	{
		// TODO: Moving platform support
        
		AddForce(new Vector2(0, Parameters.JumpMagnitude));
		_jumpIn = Parameters.JumpFrequency;
	}

    // LateUpdate() вызывается один раз в кадре, после завершения Update().
    // Любые расчеты, которые осуществляются в Update() будет завершены, при вызове LateUpdate().
    public void LateUpdate()
	{
		_jumpIn -= Time.deltaTime;
        // Задаем постоянное воздействие на игрока гравитации
		_velocity.y += Parameters.Gravity * Time.deltaTime;

        Move(Velocity * Time.deltaTime);
    }

    /// <summary>
    /// Движение персонажа
    /// </summary>
    /// <param name="deltaMovement">Шаг передвижения</param>
	private void Move(Vector2 deltaMovement)
	{
        // получаем статус или был игрок на земле
		var wasGrounded = State.IsColidingBelow;
        // сбрасываем статус
		State.Reset();

        // Проверка столкновений в awake устанавливаем true
        // В данном цикле мы изменяем в соответствии до направления движения переменную ref deltaMovement
        if (HandleCollisions)
		{
			HandlePlatforms();
			CalculateRayOrigins();

            // Если персонаж движется вниз по у и был на земле
            // Тогда его передвижения - движение по склонной поверхности
			if (deltaMovement.y < 0 && wasGrounded)
                // ref потому что в логике метода нужно будет изменить расстояние - deltaMovement
                HandleVerticalSlope(ref deltaMovement);

            // Если передвижение хоть минимальное тогда есть смысл двигать персонажа
			if (Mathf.Abs(deltaMovement.x) > .001f)
				MoveHorizontally(ref deltaMovement);

            // Движение по вертикали выполнять всегда потому что есть гравитация и оно может постоянно меняться
			MoveVertically(ref deltaMovement);

            // Пускаем лучи внутри игрока (до кожи) для проверки или ничто в него не зашло 
            // и изменяем его позицию если что то действует на него
            // вправо
			CorrectHorizontalPlacement(ref deltaMovement, true);
            // влево
			CorrectHorizontalPlacement(ref deltaMovement, false);
		}

        // После цикла получая измененную переменную  передвигаем персонажа 
		_transform.Translate(deltaMovement, Space.World);

        // ?? Задаем новую скорость которая зависит от времени перемещения
		if (Time.deltaTime > 0)
			_velocity = deltaMovement / Time.deltaTime;

        // Возвращаем меньшее из 2 чисел - скорость что мы рассчитали 
        // и MAxVelocity.x,y - которые задаем в игре в скрипте
		_velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
		_velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        //,,,,,,,,,,,,,,,,,
		if (State.IsMovingUpSlope)
			_velocity.y = 0;

        // Если стоим на платформе
		if (StandingOn != null)
		{
            // глобальная позиция платформы = позиции игрока
			_activeGlobalPlatformPoint = transform.position;
            _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
            
			if (_lastStandingOn != StandingOn)
			{
				if (_lastStandingOn != null)
					_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

				StandingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = StandingOn;
			}
			else if (StandingOn != null)
                StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
		}
		else if (_lastStandingOn != null)
		{
			_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
			_lastStandingOn = null;
		}
	}

    /// <summary>
    /// Управление платформой
    /// </summary>
	private void HandlePlatforms()
	{
		if (StandingOn != null)
		{
            // Коорднаты платформы на которой стоит персонаж в мировых координатах
			Vector3 newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);

            Vector3 moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

			if (moveDistance != Vector3.zero)
				transform.Translate(moveDistance, Space.World);

			PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
		}
		else
			PlatformVelocity = Vector3.zero;

        // Устанавливаем до следующего фрейма платформу в нулл
		StandingOn = null;
	}

    /// <summary>
    /// Контроль перемещение игрока при воздействии на него другого кинематического предмета
    /// </summary>
	private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
	{
        // Размер половины игрока
		float halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
        // 
		Vector3 rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

        // Не считаем тут толщину кожи игрока
        // Лучи всредине игрока
		if (isRight)
			rayOrigin.x -= (halfWidth - SkinWidth);
		else
			rayOrigin.x += (halfWidth - SkinWidth);

        // Направление контрольного луча
		Vector2 rayDirection = isRight ? Vector2.right : Vector2.left;
		var offset = 0f;

        // Создаем лучи внутри
		for (var i = 1; i < TotalHorizontalRays - 1; i++)
		{
            // Создаем направление вектора
            Vector2 rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
            // Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);

            //Кто ударился в нас
            RaycastHit2D raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
			if (!raycastHit)
				continue;

            // Расстояние на которое зашла платформа в нас - на єто расстояние сдвигаем игрока
			offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
		}

		deltaMovement.x += offset;
	}

    /// <summary>
    /// Определяем координаты лучей
    /// </summary>
	private void CalculateRayOrigins()
	{
        // Рассчитываем абсолютный размер коллайдера обьекта с учетом масштабирования
        // и делим на 2 для того что бы получить размер от центра коллайдера до крайней стенки по х и у
		var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;

        // Получаем центр коллайдера (offset = center)
        // Зачем умножать на масштаб непонятно,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
        var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        // получаем координаты лучей в верхней левой и остальных точках
		_raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
		_raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
		_raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);
	}

    /// <summary>
    /// Перемещение по горизонтале
    /// </summary>
	private void MoveHorizontally(ref Vector2 deltaMovement)
	{
        // движение влево/вправо
        var isGoingRight = deltaMovement.x > 0;
        // длина перемещения = длина перемещения + толщине кожи
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        // направление движение влево/вправо
        var rayDirection = isGoingRight ? Vector2.right : Vector2.left;
        // с какой точки отсчет начинать если движение влево/ вправо
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

		for (var i = 0; i < TotalHorizontalRays; i++)
		{
            // рисуем горизонтальные векторы с шагом по вертикали
			var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
			Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
            // Вывод на консоль. проверить на движение влево


			var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			if (!rayCastHit)
				continue;

			if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
				break;

			deltaMovement.x = rayCastHit.point.x - rayVector.x;
			rayDistance = Mathf.Abs(deltaMovement.x);

			if (isGoingRight)
			{
				deltaMovement.x -= SkinWidth;
				State.IsColidingRight = true;
			}
			else
			{
				deltaMovement.x += SkinWidth;
				State.IsColidingLeft = true;
			}

			if (rayDistance < SkinWidth + .0001f)
				break;
		}
	}

	private void MoveVertically(ref Vector2 deltaMovement)
	{
        // движение вниз/вверх
        var isGoingUp = deltaMovement.y > 0;
        // расстояние на которое летит луч
		var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        // направление движение луча
		var rayDirection = isGoingUp ? Vector2.up : Vector2.down;
        // откуда начинаем стрелять лучем
		Vector3 rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;
        // смещаем по х если игрок бежит
		rayOrigin.x += deltaMovement.x;

		var standingOnDistance = float.MaxValue;

        // Создаем массив лучей по оси У
		for (var i = 0; i < TotalVerticalRays; i++)
		{
            // Создаем луч смещая координаты х на равные отрезки и по оси у
			Vector2 rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            // рисуем луч для проверки
			Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            // Информация о обьекте что встетились с лучами
			RaycastHit2D raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            // Если луч несчем не сталкивается продолжаем итерацию
            if (!raycastHit)
				continue;

            //Debug.Log(raycastHit.collider.gameObject.name);


            // TODO: Непонятно данный код
            #region
            // Если мы не идем вверх
            if (!isGoingUp)
			{
                // Разница между позицией игрока и позицией соприкоснавения луча с обьектом по оси У
				float verticalDistanceToHit = _transform.position.y - raycastHit.point.y;
                // Если данное расстояние меньше за?????????
				if (verticalDistanceToHit < standingOnDistance)
				{
					standingOnDistance = verticalDistanceToHit;
                    // храним информацию обьекта на котором стоим
					StandingOn = raycastHit.collider.gameObject;
				}
			}

            // перемещение по У = самое короткое расстояние до ударения с чем либо????
            deltaMovement.y = raycastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isGoingUp)
            {
                deltaMovement.y -= SkinWidth;
                State.IsColidingAbove = true;
            }
            else
            {
                deltaMovement.y += SkinWidth;
                State.IsColidingBelow = true;
            }

            if (!isGoingUp && deltaMovement.y > .0001f)
                State.IsMovingUpSlope = true;

            if (rayDistance < SkinWidth + .0001f)
                break;
            #endregion
        }
	}

    /// <summary>
    /// Перемещение по наклонной вертикальной поверхности
    /// </summary>
    /// <param name="deltaMovement">перемещение игрока</param>
	private void HandleVerticalSlope(ref Vector2 deltaMovement)
	{
        // Средина низа модели игрока
		float center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
        // направление движения (по оси У)
		Vector2 direction = Vector2.down;

        // Расстояние (треугольник половины ширины коллайдера игрока на танг.75 градусов - второй катет)
		float slopeDistance = SlopeLimitTangant * (_raycastBottomRight.x - center);
        // Координаты средины низа модели игрока
		Vector2 slopeRayVector = new Vector2(center, _raycastBottomLeft.y);
        
		Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.yellow);
        
        // Получаем обьект на котором находится игрок
        // Длина луча вниз должна быть больше чем половина ширины коллайдера игрока
        // Так как луч выпускается с центра модели а игрок на платформе соприкосается краем коллайдера
		RaycastHit2D raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        //???????????????проверка на нулл?????????????
        if (!raycastHit)
			return;

        // ReSharper disable CompareOfFloatsByEqualityOperator 

        // Sign(f) - Return value is 1 when f is positive or zero, -1 when f is negative.
        // raycastHit.normal - Нормаль к точке соприкоснавения луча с платформой
        bool isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
		if (!isMovingDownSlope)
			return;

        // Угол между нормалью к платформе в  точке удара луча и вертикалью
		float angle = Vector2.Angle(raycastHit.normal, Vector2.up);
        // Если угол очень маленький
		if (Mathf.Abs(angle) < .0001f)
			return;

        // устанавливаем состояние игрока
		State.IsMovingDownSlope = true;
		State.SlopeAngle = angle;

        // Расстояние между срединой низа игрока и платформой
		deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
	}
	private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
	{
        // Если угол 89,5-90,5
		if (Mathf.RoundToInt(angle) == 90)
			return false;

        // если угол наклона больше чем предельный, тогда невозможно поднятся на горку
		if (angle > Parameters.SlopeLimit)
		{
			deltaMovement.x = 0;
			return true;
		}


        //TODO: РАЗОБРАТЬ ДОКОНЦА
		if (deltaMovement.y > .07f)
			return true;

		deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
		deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
		State.IsMovingUpSlope = true;
		State.IsColidingBelow = true;
		return true;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
		if (parameters == null)
			return;

		_overrideParameters = parameters.Parameters;
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
		if (parameters == null)
			return;

		_overrideParameters = null;
	}
}