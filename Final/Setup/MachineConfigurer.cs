using WJLCS.Enigma;

namespace WJLCS.Setup {
	/// <summary>
	/// The interpreter class that runs the Enigma Machine.
	/// </summary>
	public class MachineConfigurer {

		#region Fields

		/// <summary>
		/// The configurer for the Enigma Machine letterset.
		/// </summary>
		private readonly LetterSetConfigurer letterSetConfig;
		/// <summary>
		/// The configurer for the Enigma Machine plugboard.
		/// </summary>
		private readonly PlugboardConfigurer plugboardConfig;
		/// <summary>
		/// The configurer for the Enigma Machine rotors.
		/// </summary>
		private readonly RotorConfigurer rotorConfig;

		/// <summary>
		/// Gets the configured Enigma Machine.
		/// </summary>
		public Machine Machine { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the Enigma Machine <see cref="MachineConfigurer"/>.
		/// </summary>
		public MachineConfigurer(LetterSetConfigurer letterSetConfig,
							PlugboardConfigurer plugboardConfig,
							RotorConfigurer rotorConfig)
		{
			this.letterSetConfig = letterSetConfig;
			this.plugboardConfig = plugboardConfig;
			this.rotorConfig = rotorConfig;
			SetupMachineIfReady();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the setup status of the machine.
		/// </summary>
		public MachineStatus Status {
			get {
				return new MachineStatus {
					IsSetup = Machine != null,

					LetterSetFile	= letterSetConfig.File,
					LetterSetLoaded	= letterSetConfig.LetterSet != null,
					LetterSetHash	= letterSetConfig.LetterSet?.GetHashCode() ?? 0,
					LetterCount		= letterSetConfig.LetterSet?.Count ?? 0,

					PlugboardFile	= plugboardConfig.File,
					PlugboardLoaded	= plugboardConfig.Steckering != null,
					PlugboardHash	= plugboardConfig.Steckering?.GetHashCode() ?? 0,

					RotorKeysFile	= rotorConfig.File,
					RotorKeysLoaded	= rotorConfig.RotorKeys != null,
					RotorKeysHash	= rotorConfig.RotorKeys?.GetHashCode() ?? 0,
					RotorCount		= rotorConfig.RotorKeys?.Count ?? 0,
					RotorKeys		= rotorConfig.RotorKeys.ToArray(),
				};
			}
		}
		/// <summary>
		/// Gets if the Engma Machine is setup.
		/// </summary>
		public bool IsSetup => Machine != null;

		#endregion
		
		#region Setup

		/// <summary>
		/// Sets up the Enigma Machine if the letterset and steckering is ready.<para/>
		/// Otherwise destroys it.
		/// </summary>
		/// <returns>True if the machine was setup.</returns>
		public bool SetupMachineIfReady() {
			if (letterSetConfig.LetterSet != null && plugboardConfig.Steckering != null &&
				rotorConfig.RotorKeys != null)
			{
				Machine = new Machine(new SetupArgs {
					LetterSet = letterSetConfig.LetterSet,
					Steckering = plugboardConfig.Steckering,
					RotorKeys = rotorConfig.RotorKeys,
				});
				return true;
			}
			else {
				Machine = null;
				return false;
			}
		}

		#endregion
	}
}
