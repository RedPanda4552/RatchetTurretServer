using System;
using System.Threading;

namespace RatchetTurretServer
{
    public class Program
    {
        private static readonly string RATCHET_2_PAL_SERIAL = "SCES-51607";
        private static readonly UInt32 OFF_0 = 0x0280202d;
        private static readonly UInt32 OFF_4 = 0x3c0140d0;
        private static readonly UInt32 OFF_8 = 0x44810800;
        private static readonly UInt32 OFF_10 = 0x46000836;
        private static readonly UInt32 FIX = 0x3c013f80;

        static void Main(string[] args)
        {
            new Program();
        }

        IntPtr pine;
        UInt32 turretAddr = 0;

        public Program()
        {
            Init();
            Exec();
            Exit();
        }

        private void Init()
        {
            pine = PINE.NewPcsx2();
        }

        private void Exec()
        {
            Console.WriteLine("================================================================================");
            Console.WriteLine("Ratchet Turret Server");
            Console.WriteLine("================================================================================");
            
            while (Heartbeat())
            {
                Console.WriteLine("Scanning Ratchet memory for turret code... This may take a minute...");
                turretAddr = ScanForTurretCode();

                if (turretAddr == 0xffffffff)
                {
                    Console.WriteLine("Could not find turret code, exiting");
                    return;
                }

                Console.WriteLine("Turret code found at 0x{0:X}! Patching...", turretAddr);
                PINE.Write32(pine, turretAddr + 4, FIX, false);
                
                Console.WriteLine("Ratchet Turret Server will continue to monitor for changes to turret memory.");
                Console.WriteLine("To exit, either close the game, or hit Ctrl + C in this console window.\n");

                do
                {    
                    Thread.Sleep(500);
                }
                while (ScanAtAddress(turretAddr));

                if (IsMemoryCleared(turretAddr))
                {
                    Console.WriteLine("PCSX2 appears to have switched games. Exiting.");
                    return;
                }
                
                Console.WriteLine("Turret code has moved.");
            }

            Console.WriteLine("PCSX2 is not running, or the game is not Ratchet and Clank 2: Locked and Loaded (PAL). Exiting.");
        }

        private void Exit()
        {
            PINE.DeletePcsx2(pine);
            // Delay exit a little bit just so people get
            // a chance to read any messages in the console
            Thread.Sleep(2000);
        }

        private Boolean Heartbeat()
        {
            return PINE.Status(pine, false) == PINE.EmuStatus.Running && PINE.GetGameID(pine, false).Equals(RATCHET_2_PAL_SERIAL);
        }

        // Check if the Ratchet-critical addresses have been cleared to 0.
        // PCSX2 dumps EE memory when switching games before it actually
        // updates the serial, so we can use this to tell the difference
        // between Ratchet just shuffling around turret code, and a VM
        // reset.
        private Boolean IsMemoryCleared(UInt32 address)
        {
            return PINE.Read32(pine, address, false) == 0
                && PINE.Read32(pine, address + 0x4, false) == 0
                && PINE.Read32(pine, address + 0x8, false) == 0
                && PINE.Read32(pine, address + 0x10, false) == 0;
        }

        private Boolean ScanAtAddress(UInt32 address)
        {
            UInt32 value = 0, off4 = 0, off8 = 0, off10 = 0;
            value = PINE.Read32(pine, address, false);

            if (value != OFF_0)
            {
                return false;
            }

            off4 = PINE.Read32(pine, address + 0x4, false);

            if (off4 != OFF_4 && off4 != FIX)
            {
                return false;
            }
            
            off8 = PINE.Read32(pine, address + 0x8, false);

            if (off8 != OFF_8)
            {
                return false;
            }
            
            off10 = PINE.Read32(pine, address + 0x10, false);

            if (off10 != OFF_10)
            {
                return false;
            }
            
            return true;
        }

        // Indvidual scans are fragmented to try and optimize for where the
        // turret code is most often found.
        private UInt32 ScanForTurretCode()
        {
            // Turret code seems to always reside here during gameplay
            for (UInt32 address = 0x380000; address < 0x400000; address += 4)
            {
                if (ScanAtAddress(address))
                {
                    return address;
                }
            }

            // When the game first boots, turret code resides somewhere in here
            for (UInt32 address = 0x300000; address < 0x380000; address += 4)
            {
                if (ScanAtAddress(address))
                {
                    return address;
                }
            }
            
            // If still not found, check lower memory first
            for (UInt32 address = 0; address < 0x300000; address += 4)
            {
                if (ScanAtAddress(address))
                {
                    return address;
                }
            }
            
            // Lastly, run through upper memory and see if its up there for some reason
            for (UInt32 address = 0x400000; address < 0x2000000; address += 4)
            {
                if (ScanAtAddress(address))
                {
                    return address;
                }
            }
            
            return 0xffffffff;
        }
    }
}
