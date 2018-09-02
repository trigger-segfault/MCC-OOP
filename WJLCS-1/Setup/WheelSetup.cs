
namespace WJLCS {
	/// <summary>
	/// An initial setup for a central wheel.
	/// </summary>
	public class WheelSetup {

		#region Properties

		/// <summary>
		/// Gets or sets the wheel to use.
		/// </summary>
		public IWheel Wheel { get; }

		/// <summary>
		/// Gets or sets the starting position of the wheel.
		/// </summary>
		public int InitialPosition { get; }

		/// <summary>
		/// Gets or sets the turnover position of the wheel.
		/// </summary>
		public int TurnoverPosition { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a wheel setup with the specified wheel and positions.
		/// </summary>
		/// <param name="wheel">The wheel to use.</param>
		/// <param name="initialPosition">The initial position of the wheel.</param>
		/// <param name="turnoverPosition">The turnover position of the wheel.</param>
		public WheelSetup(IWheel wheel, int initialPosition = 0, int turnoverPosition = 0) {
			Wheel = wheel;
			InitialPosition = initialPosition;
			TurnoverPosition = turnoverPosition;
		}

		#endregion
	}
}
