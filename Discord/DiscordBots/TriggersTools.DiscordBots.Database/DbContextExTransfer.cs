using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TriggersTools.DiscordBots.Database {
	/// <summary>
	/// The static design-time transfer class for <see cref="DbContextEx"/>s.
	/// </summary>
	public static class DbContextExTransfer {

		#region Transfer

		public static void Transfer(IDbContextExFactory factory, string srcConfigurationType, string destConfigurationType) {
			Console.WriteLine($"Transfer '{srcConfigurationType}' => '{destConfigurationType}'");
			Console.Write("Is this correct? (y/n): ");
			char c;
			do {
				c = char.ToLower((char) Console.Read());
			} while (c != 'y' && c != 'n');
			Console.WriteLine();
			if (c != 'y') {
				Console.WriteLine("Cancelled!");
				return;
			}
			Console.WriteLine("Opening Databases...");
			MethodInfo method = typeof(DbContextExTransfer)
				.GetMethod(nameof(TransferTable), BindingFlags.NonPublic | BindingFlags.Static);
			using (var dbSrc = GetDb(factory, srcConfigurationType))
			using (var dbDest = GetDb(factory, destConfigurationType)) {
				dbDest.Database.EnsureCreated();
				var tables = DbReflection.GetTables(dbSrc, dbDest);

				foreach (var mt in tables) {
					if (mt.Tables[1].Cast<object>().Any())
						throw new ArgumentException($"Can only transfer to an empty database!\n" +
													$"Table \"{mt.TableName} is not empty!");
				}

				foreach (var mt in tables) {
					Console.WriteLine($"Transfering Table: '{mt.TableName}");
					MethodInfo generic = mt.MakeGenericMethod(method);
					generic.Invoke(null, new object[] { mt.Tables[0], mt.Tables[1] });
				}

				dbDest.SaveChanges();
			}
			Console.WriteLine();
			Console.WriteLine("Complete!");
		}

		private static void TransferTable<T>(DbSet<T> srcTable, DbSet<T> destTable) where T : class {
			destTable.AddRangeAsync(srcTable);
		}

		public static void ClearDatabase(IDbContextExFactory factory, string configurationType) {
			using (var db = GetDb(factory, configurationType)) {
				foreach (var t in DbReflection.GetTables(db)) {
					db.Database.ExecuteSqlCommand($"TRUNCATE TABLE [{t.TableName}]");
				}
				db.SaveChanges();
			}
		}

		#endregion

		#region Databases

		private static DbContextEx GetDb(IDbContextExFactory factory, string configurationType) {
			DbContextEx db = factory.CreateDbContext();
			db.ConfigurationType = configurationType;
			db.DisableEncryption = true;
			return db;
		}

		#endregion
	}
}
