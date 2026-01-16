using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Skip if data already exists
        if (await context.Equipment.AnyAsync() || await context.Rooms.AnyAsync() || await context.Students.AnyAsync())
        {
            return;
        }

        // Seed Equipment (lookup table)
        var equipment = new[]
        {
            new Equipment { Name = "Steinway Grand Piano Model D", Type = EquipmentType.GrandPiano, Description = "Concert grand piano, 274 cm" },
            new Equipment { Name = "Yamaha Grand Piano C3X", Type = EquipmentType.GrandPiano, Description = "Studio grand piano, 186 cm" },
            new Equipment { Name = "Yamaha Upright Piano U1", Type = EquipmentType.UprightPiano, Description = "Professional upright piano" },
            new Equipment { Name = "Kawai Upright Piano K-300", Type = EquipmentType.UprightPiano, Description = "Studio upright piano" },
            new Equipment { Name = "Pearl Export Drum Kit", Type = EquipmentType.Drums, Description = "5-piece drum set with cymbals" },
            new Equipment { Name = "Fender Guitar Amplifier", Type = EquipmentType.Amplifier, Description = "50W combo amp" },
            new Equipment { Name = "Shure SM58 Microphone", Type = EquipmentType.Microphone, Description = "Dynamic vocal microphone" },
            new Equipment { Name = "Manhasset Music Stand", Type = EquipmentType.MusicStand, Description = "Professional music stand" },
            new Equipment { Name = "Stradivarius Violin (Copy)", Type = EquipmentType.Violin, Description = "Professional violin" },
            new Equipment { Name = "Yamaha Cello VC20G", Type = EquipmentType.Cello, Description = "Student cello" },
            new Equipment { Name = "Fender Stratocaster", Type = EquipmentType.Guitar, Description = "Electric guitar" },
            new Equipment { Name = "Bach Stradivarius Trumpet", Type = EquipmentType.Trumpet, Description = "Professional trumpet" },
            new Equipment { Name = "Yamaha Alto Saxophone YAS-280", Type = EquipmentType.Saxophone, Description = "Student alto saxophone" },
            new Equipment { Name = "Gemeinhardt Flute 3SHB", Type = EquipmentType.Flute, Description = "Intermediate flute" }
        };
        await context.Equipment.AddRangeAsync(equipment);
        await context.SaveChangesAsync();

        // Seed Instructors
        var instructors = new[]
        {
            new Instructor { FirstName = "Maria", LastName = "Chen", Email = "maria.chen@conservatory.edu" },
            new Instructor { FirstName = "Robert", LastName = "Williams", Email = "robert.williams@conservatory.edu" },
            new Instructor { FirstName = "Elena", LastName = "Petrova", Email = "elena.petrova@conservatory.edu" },
            new Instructor { FirstName = "James", LastName = "Thompson", Email = "james.thompson@conservatory.edu" }
        };
        await context.Instructors.AddRangeAsync(instructors);
        await context.SaveChangesAsync();

        // Seed Rooms - 25 total (15 small, 6 medium, 4 large)
        var rooms = new List<Room>();
        
        // 15 Small rooms
        for (int i = 1; i <= 15; i++)
        {
            rooms.Add(new Room 
            { 
                Name = $"Room {100 + i}", 
                Type = RoomType.Small, 
                Capacity = 1, 
                IsSoundproof = i % 3 == 0 // Every 3rd room is soundproof
            });
        }
        
        // 6 Medium rooms
        for (int i = 1; i <= 6; i++)
        {
            rooms.Add(new Room 
            { 
                Name = $"Room {200 + i}", 
                Type = RoomType.Medium, 
                Capacity = 4, 
                IsSoundproof = i % 2 == 0
            });
        }
        
        // 4 Large rooms
        for (int i = 1; i <= 4; i++)
        {
            rooms.Add(new Room 
            { 
                Name = $"Room {300 + i}", 
                Type = RoomType.Large, 
                Capacity = 8, 
                IsSoundproof = true // All large rooms are soundproof
            });
        }
        
        await context.Rooms.AddRangeAsync(rooms);
        await context.SaveChangesAsync();

        // Seed RoomEquipments - assign equipment to rooms
        var roomEquipments = new List<RoomEquipment>();
        
        // Small rooms - diverse instruments
        // Room 101: Grand Piano
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[0].Id, EquipmentId = equipment[0].Id, Quantity = 1 }); // Steinway Grand
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[0].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 102: Upright Piano
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[1].Id, EquipmentId = equipment[2].Id, Quantity = 1 }); // Upright piano
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[1].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 103: Guitar
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[2].Id, EquipmentId = equipment[10].Id, Quantity = 1 }); // Guitar
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[2].Id, EquipmentId = equipment[5].Id, Quantity = 1 }); // Amplifier
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[2].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 104: Violin
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[3].Id, EquipmentId = equipment[8].Id, Quantity = 1 }); // Violin
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[3].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 105: Cello
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[4].Id, EquipmentId = equipment[9].Id, Quantity = 1 }); // Cello
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[4].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 106: Upright Piano
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[5].Id, EquipmentId = equipment[3].Id, Quantity = 1 }); // Upright piano
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[5].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 107: Flute
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[6].Id, EquipmentId = equipment[13].Id, Quantity = 1 }); // Flute
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[6].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 108: Grand Piano
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[7].Id, EquipmentId = equipment[1].Id, Quantity = 1 }); // Yamaha Grand
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[7].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 109: Violin
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[8].Id, EquipmentId = equipment[8].Id, Quantity = 1 }); // Violin
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[8].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 110: Guitar
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[9].Id, EquipmentId = equipment[10].Id, Quantity = 1 }); // Guitar
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[9].Id, EquipmentId = equipment[5].Id, Quantity = 1 }); // Amplifier
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[9].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Rooms 111-113: Drums (soundproof)
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[10].Id, EquipmentId = equipment[4].Id, Quantity = 1 }); // Drum kit
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[10].Id, EquipmentId = equipment[5].Id, Quantity = 1 }); // Amplifier
        
        // Room 112: Trumpet (soundproof)
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[11].Id, EquipmentId = equipment[11].Id, Quantity = 1 }); // Trumpet
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[11].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 113: Saxophone (soundproof)
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[12].Id, EquipmentId = equipment[12].Id, Quantity = 1 }); // Saxophone
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[12].Id, EquipmentId = equipment[7].Id, Quantity = 1 }); // Music stand
        
        // Room 114: Drums (soundproof)
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[13].Id, EquipmentId = equipment[4].Id, Quantity = 1 }); // Drum kit
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[13].Id, EquipmentId = equipment[5].Id, Quantity = 1 }); // Amplifier
        
        // Room 115: ALL instruments
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[0].Id, Quantity = 1 }); // Grand piano
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[4].Id, Quantity = 1 }); // Drums
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[5].Id, Quantity = 1 }); // Amplifier
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[6].Id, Quantity = 2 }); // Microphones
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[7].Id, Quantity = 4 }); // Music stands
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[8].Id, Quantity = 1 }); // Violin
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[9].Id, Quantity = 1 }); // Cello
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[10].Id, Quantity = 1 }); // Guitar
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[11].Id, Quantity = 1 }); // Trumpet
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[12].Id, Quantity = 1 }); // Saxophone
        roomEquipments.Add(new RoomEquipment { RoomId = rooms[14].Id, EquipmentId = equipment[13].Id, Quantity = 1 }); // Flute
        
        // Medium rooms 201-202: ALL instruments
        for (int i = 15; i < 17; i++)
        {
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[0].Id, Quantity = 1 }); // Grand piano
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[4].Id, Quantity = 1 }); // Drums
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[5].Id, Quantity = 2 }); // Amplifiers
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[6].Id, Quantity = 2 }); // Microphones
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[7].Id, Quantity = 4 }); // Music stands
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[8].Id, Quantity = 1 }); // Violin
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[9].Id, Quantity = 1 }); // Cello
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[10].Id, Quantity = 1 }); // Guitar
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[11].Id, Quantity = 1 }); // Trumpet
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[12].Id, Quantity = 1 }); // Saxophone
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[13].Id, Quantity = 1 }); // Flute
        }
        
        // Medium rooms 203-206: ensemble setup
        for (int i = 17; i < 21; i++)
        {
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[3].Id, Quantity = 1 }); // Upright piano
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[7].Id, Quantity = 4 }); // Music stands
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[6].Id, Quantity = 2 }); // Microphones
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[8].Id, Quantity = 1 }); // Violin
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[9].Id, Quantity = 1 }); // Cello
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[12].Id, Quantity = 1 }); // Flute
        }
        
        // Large rooms - full band setup
        for (int i = 21; i < 25; i++)
        {
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[0].Id, Quantity = 1 }); // Grand piano
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[4].Id, Quantity = 1 }); // Drum kit
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[5].Id, Quantity = 2 }); // Amplifiers
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[6].Id, Quantity = 4 }); // Microphones
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[7].Id, Quantity = 8 }); // Music stands
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[8].Id, Quantity = 2 }); // Violin
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[9].Id, Quantity = 1 }); // Cello
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[10].Id, Quantity = 1 }); // Guitar
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[11].Id, Quantity = 1 }); // Trumpet
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[12].Id, Quantity = 1 }); // Saxophone
            roomEquipments.Add(new RoomEquipment { RoomId = rooms[i].Id, EquipmentId = equipment[13].Id, Quantity = 1 }); // Flute
        }
        
        await context.RoomEquipments.AddRangeAsync(roomEquipments);
        await context.SaveChangesAsync();

        // Seed Students
        var students = new[]
        {
            // Performance Majors (20 hours quota)
            new Student 
            { 
                FirstName = "Sophia", LastName = "Anderson", 
                Email = "sophia.anderson@student.edu", StudentNumber = "PM2024001",
                Program = StudentProgram.PerformanceMajor, PrimaryInstrument = InstrumentType.Piano,
                InstructorId = instructors[0].Id
            },
            new Student 
            { 
                FirstName = "Michael", LastName = "Brown", 
                Email = "michael.brown@student.edu", StudentNumber = "PM2024002",
                Program = StudentProgram.PerformanceMajor, PrimaryInstrument = InstrumentType.Violin,
                InstructorId = instructors[0].Id
            },
            new Student 
            { 
                FirstName = "Emma", LastName = "Davis", 
                Email = "emma.davis@student.edu", StudentNumber = "PM2024003",
                Program = StudentProgram.PerformanceMajor, PrimaryInstrument = InstrumentType.Cello,
                InstructorId = instructors[1].Id
            },
            new Student 
            { 
                FirstName = "Alexander", LastName = "Martinez", 
                Email = "alex.martinez@student.edu", StudentNumber = "PM2024004",
                Program = StudentProgram.PerformanceMajor, PrimaryInstrument = InstrumentType.Trumpet,
                InstructorId = instructors[2].Id
            },
            new Student 
            { 
                FirstName = "Isabella", LastName = "Garcia", 
                Email = "isabella.garcia@student.edu", StudentNumber = "PM2024005",
                Program = StudentProgram.PerformanceMajor, PrimaryInstrument = InstrumentType.Voice,
                InstructorId = instructors[2].Id
            },
            
            // Education Majors (10 hours quota)
            new Student 
            { 
                FirstName = "William", LastName = "Johnson", 
                Email = "william.johnson@student.edu", StudentNumber = "EM2024001",
                Program = StudentProgram.EducationMajor, PrimaryInstrument = InstrumentType.Piano,
                InstructorId = instructors[0].Id
            },
            new Student 
            { 
                FirstName = "Olivia", LastName = "Wilson", 
                Email = "olivia.wilson@student.edu", StudentNumber = "EM2024002",
                Program = StudentProgram.EducationMajor, PrimaryInstrument = InstrumentType.Flute,
                InstructorId = instructors[1].Id
            },
            new Student 
            { 
                FirstName = "Daniel", LastName = "Taylor", 
                Email = "daniel.taylor@student.edu", StudentNumber = "EM2024003",
                Program = StudentProgram.EducationMajor, PrimaryInstrument = InstrumentType.Guitar,
                InstructorId = instructors[3].Id
            },
            
            // Minors (5 hours quota)
            new Student 
            { 
                FirstName = "Emily", LastName = "Moore", 
                Email = "emily.moore@student.edu", StudentNumber = "MN2024001",
                Program = StudentProgram.Minor, PrimaryInstrument = InstrumentType.Piano,
                InstructorId = instructors[0].Id
            },
            new Student 
            { 
                FirstName = "James", LastName = "Jackson", 
                Email = "james.jackson@student.edu", StudentNumber = "MN2024002",
                Program = StudentProgram.Minor, PrimaryInstrument = InstrumentType.Saxophone,
                InstructorId = instructors[3].Id
            },
            
            // Student with no-show history (penalized)
            new Student 
            { 
                FirstName = "Lucas", LastName = "White", 
                Email = "lucas.white@student.edu", StudentNumber = "PM2024006",
                Program = StudentProgram.PerformanceMajor, PrimaryInstrument = InstrumentType.Drums,
                InstructorId = instructors[1].Id,
                NoShowCount = 3,
                QuotaPenaltyHours = 5 // Reduced from 20 to 15 hours
            }
        };
        
        await context.Students.AddRangeAsync(students);
        await context.SaveChangesAsync();

        // Seed some Bookings
        var now = DateTime.Now;
        var bookings = new[]
        {
            // Current week bookings
            new Booking
            {
                StudentId = students[0].Id,
                RoomId = rooms[0].Id, // Room 101 - Grand Piano
                StartTime = now.Date.AddDays(1).AddHours(9),
                EndTime = now.Date.AddDays(1).AddHours(11),
                Status = BookingStatus.Confirmed,
                Purpose = BookingPurpose.RegularPractice,
                CreatedAt = now.AddDays(-2)
            },
            new Booking
            {
                StudentId = students[1].Id,
                RoomId = rooms[15].Id, // Room 201 - Medium room
                StartTime = now.Date.AddDays(1).AddHours(14),
                EndTime = now.Date.AddDays(1).AddHours(16),
                Status = BookingStatus.Confirmed,
                Purpose = BookingPurpose.EnsembleRehearsal,
                CreatedAt = now.AddDays(-3)
            },
            new Booking
            {
                StudentId = students[4].Id,
                RoomId = rooms[21].Id, // Room 301 - Large room
                StartTime = now.Date.AddDays(2).AddHours(10),
                EndTime = now.Date.AddDays(2).AddHours(13),
                Status = BookingStatus.Confirmed,
                Purpose = BookingPurpose.RecitalPrep,
                RequiresApproval = true,
                IsApproved = true,
                ApprovedByInstructorId = instructors[2].Id,
                ApprovedAt = now.AddDays(-1),
                CreatedAt = now.AddDays(-4)
            },
            
            // Past booking - completed
            new Booking
            {
                StudentId = students[0].Id,
                RoomId = rooms[1].Id,
                StartTime = now.Date.AddDays(-3).AddHours(10),
                EndTime = now.Date.AddDays(-3).AddHours(12),
                Status = BookingStatus.Completed,
                Purpose = BookingPurpose.RegularPractice,
                CheckedInAt = now.Date.AddDays(-3).AddHours(10).AddMinutes(5),
                CreatedAt = now.AddDays(-5)
            },
            
            // Past booking - no show (from penalized student)
            new Booking
            {
                StudentId = students[10].Id, // Lucas White
                RoomId = rooms[12].Id,
                StartTime = now.Date.AddDays(-5).AddHours(14),
                EndTime = now.Date.AddDays(-5).AddHours(16),
                Status = BookingStatus.NoShow,
                Purpose = BookingPurpose.RegularPractice,
                IsNoShow = true,
                CreatedAt = now.AddDays(-7)
            },
            
            // Cancelled booking
            new Booking
            {
                StudentId = students[5].Id,
                RoomId = rooms[5].Id,
                StartTime = now.Date.AddDays(-1).AddHours(15),
                EndTime = now.Date.AddDays(-1).AddHours(17),
                Status = BookingStatus.Cancelled,
                Purpose = BookingPurpose.RegularPractice,
                CreatedAt = now.AddDays(-4),
                CancelledAt = now.AddDays(-2)
            }
        };
        
        await context.Bookings.AddRangeAsync(bookings);
        await context.SaveChangesAsync();
        
        // Update TotalLoggedHours for students with completed bookings
        students[0].TotalLoggedHours = 2;
        await context.SaveChangesAsync();
    }
}
