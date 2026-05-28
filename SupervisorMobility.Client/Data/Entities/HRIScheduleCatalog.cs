namespace SupervisorMobility.Client.Data.Entities
{
    public enum HriScheduleItemType
    {
        WeeklySsvReview = 1,
        OperatorFirstShiftReview = 2,
        OperatorSecondShiftReview = 3,
        OperatorThirdShiftReview = 4,
        SafetyAndConditionsCheck = 5,
        MechanicalCheck = 6,
        ElectricalCheck = 7,
        OperationControlsCheck = 8
    }

    public class HriScheduleTemplate
    {
        public int ItemNumber { get; set; }
        public HriScheduleItemType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ReviewMethod { get; set; } = string.Empty;
        public string Criteria { get; set; } = string.Empty;
        public string ActionIfFail { get; set; } = string.Empty;
        public int SuggestedStatus { get; set; }
    }

    public static class HRIScheduleCatalog
    {
        public static IReadOnlyList<HriScheduleTemplate> Templates { get; } = new List<HriScheduleTemplate>
        {
            new HriScheduleTemplate
            {
                ItemNumber = 1,
                Type = HriScheduleItemType.MechanicalCheck,
                Title = "Fugas de aceite",
                ReviewMethod = "Revision visual",
                Criteria = "No manchas en piso",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 1
            },
            new HriScheduleTemplate
            {
                ItemNumber = 2,
                Type = HriScheduleItemType.MechanicalCheck,
                Title = "Horquillas",
                ReviewMethod = "Revision manual",
                Criteria = "Cerrados y que funcionen bien",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 1
            },
            new HriScheduleTemplate
            {
                ItemNumber = 3,
                Type = HriScheduleItemType.SafetyAndConditionsCheck,
                Title = "Chasis y contrapeso",
                ReviewMethod = "Revision manual",
                Criteria = "Sin golpes",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 4,
                Type = HriScheduleItemType.SafetyAndConditionsCheck,
                Title = "Cristales de cabina",
                ReviewMethod = "Revision visual",
                Criteria = "No quebrados",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 3
            },
            new HriScheduleTemplate
            {
                ItemNumber = 8,
                Type = HriScheduleItemType.SafetyAndConditionsCheck,
                Title = "Cabina y proteccion superior",
                ReviewMethod = "Revision visual y manual",
                Criteria = "Sin dano ni flojo",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 4
            },
            new HriScheduleTemplate
            {
                ItemNumber = 9,
                Type = HriScheduleItemType.SafetyAndConditionsCheck,
                Title = "Extintor",
                ReviewMethod = "Revision visual",
                Criteria = "Aguja en verde, con sello y vigente",
                ActionIfFail = "Reportar a proveedor/MTTO",
                SuggestedStatus = 6
            },
            new HriScheduleTemplate
            {
                ItemNumber = 10,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Conector de bateria",
                ReviewMethod = "Revision visual",
                Criteria = "Sin golpes e insertado a tope",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 11,
                Type = HriScheduleItemType.OperationControlsCheck,
                Title = "Volante",
                ReviewMethod = "Revision visual",
                Criteria = "Sin danos",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 1
            },
            new HriScheduleTemplate
            {
                ItemNumber = 12,
                Type = HriScheduleItemType.OperationControlsCheck,
                Title = "Asiento y cinturon",
                ReviewMethod = "Revision visual y manual",
                Criteria = "Sin dano y funcional",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 13,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Display de bateria",
                ReviewMethod = "Revision visual",
                Criteria = "Que encienda, sin danos",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 3
            },
            new HriScheduleTemplate
            {
                ItemNumber = 14,
                Type = HriScheduleItemType.OperationControlsCheck,
                Title = "Switch de ignicion",
                ReviewMethod = "Revision visual y manual",
                Criteria = "Sin dano",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 15,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Display de montacargas",
                ReviewMethod = "Revision visual",
                Criteria = "Que encienda, sin danos",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 3
            },
            new HriScheduleTemplate
            {
                ItemNumber = 16,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Claxon y alarma de reversa",
                ReviewMethod = "Revision auditiva/manual",
                Criteria = "Sonido fuerte",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 4
            },
            new HriScheduleTemplate
            {
                ItemNumber = 17,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Blue point frontal y trasero",
                ReviewMethod = "Revision visual y manual",
                Criteria = "Que encienda",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 18,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Torreta ambar",
                ReviewMethod = "Revision visual",
                Criteria = "Sin danos, que destelle",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 1
            },
            new HriScheduleTemplate
            {
                ItemNumber = 19,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Luz de reversa",
                ReviewMethod = "Revision visual",
                Criteria = "Sin danos, que encienda",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 20,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Luz de stop",
                ReviewMethod = "Revision visual y manual",
                Criteria = "Que encienda al pisar freno",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 21,
                Type = HriScheduleItemType.ElectricalCheck,
                Title = "Faros",
                ReviewMethod = "Revision visual",
                Criteria = "Sin danos, que enciendan",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 3
            },
            new HriScheduleTemplate
            {
                ItemNumber = 22,
                Type = HriScheduleItemType.SafetyAndConditionsCheck,
                Title = "Espejos retrovisores",
                ReviewMethod = "Revision visual",
                Criteria = "Limpios y sin dano",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 1
            },
            new HriScheduleTemplate
            {
                ItemNumber = 23,
                Type = HriScheduleItemType.OperationControlsCheck,
                Title = "Horometro",
                ReviewMethod = "Revision visual",
                Criteria = "Lectura visible",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 3
            },
            new HriScheduleTemplate
            {
                ItemNumber = 24,
                Type = HriScheduleItemType.OperationControlsCheck,
                Title = "Acelerador",
                ReviewMethod = "Revision manual",
                Criteria = "Arranque sin estirones",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 4
            },
            new HriScheduleTemplate
            {
                ItemNumber = 25,
                Type = HriScheduleItemType.MechanicalCheck,
                Title = "Mastil y desplazador",
                ReviewMethod = "Revision visual y manual",
                Criteria = "Sin atorones y sin juego",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 2
            },
            new HriScheduleTemplate
            {
                ItemNumber = 26,
                Type = HriScheduleItemType.OperationControlsCheck,
                Title = "Palancas de mando finger tip",
                ReviewMethod = "Revision visual y manual",
                Criteria = "Funcionen correctamente y sin danos",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 3
            },
            new HriScheduleTemplate
            {
                ItemNumber = 27,
                Type = HriScheduleItemType.MechanicalCheck,
                Title = "Fugas de aceite",
                ReviewMethod = "Revision visual",
                Criteria = "Sin manchas en piso",
                ActionIfFail = "Reportar a MTTO",
                SuggestedStatus = 6
            }
        };

        public static HriScheduleTemplate GetRandomTemplate(Random random)
        {
            if (Templates.Count == 0)
            {
                return new HriScheduleTemplate
                {
                    ItemNumber = 0,
                    Type = HriScheduleItemType.WeeklySsvReview,
                    Title = "HRI",
                    ReviewMethod = "Revision visual",
                    Criteria = "Cumple",
                    ActionIfFail = "Reportar",
                    SuggestedStatus = 1
                };
            }

            return Templates[random.Next(Templates.Count)];
        }
    }
}
