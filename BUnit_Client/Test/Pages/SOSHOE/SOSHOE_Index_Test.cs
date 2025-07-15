using BUnit_Client.Instances;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.SOSHOE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUnit_Client.Test.Pages.SOSHOE
{
    public class SOSHOE_Index_Test
    {
        [Fact]
        public void SOSHOEIndex_Renders_HOE_Card_And_Buttons_For_Logged_User()
        {
            // Arrange
            var ctx = new GlobalInstance(isLogged: true, userType: 2);
           
            // Act
            var cut = ctx.RenderComponent<SOSHOEIndex>();
            // Assert
            // Verifica que la tarjeta HOE existe
            Assert.Contains("SOS", cut.Markup);
            // Verifica que existen los botones "View all" y "Create" para HOE
            Assert.Contains("href=\"soshoe/Hub\"", cut.Markup);
            Assert.Contains("href=\"soshoe/Hub/Create\"", cut.Markup);
        }

        [Fact]
        public void SOSHOEIndex_Renders_Admin_Cards_And_Buttons()
        {
            // Arrange
          
            var ctx = new GlobalInstance(isLogged: true, userType: 1);

            // Act
            var cut = ctx.RenderComponent<SOSHOEIndex>();
            // Assert
            // Verifica que existen las tarjetas y botones de admin
            Assert.Contains("H/S A", cut.Markup);
            Assert.Contains("href=\"soshoe/Analysis\"", cut.Markup);
            Assert.Contains("href=\"soshoe/Generate/Analysis\"", cut.Markup);

            Assert.Contains("H/S C", cut.Markup);
            Assert.Contains("href=\"soshoe/Combination\"", cut.Markup);
            Assert.Contains("href=\"soshoe/Generate/Combination\"", cut.Markup);

            Assert.Contains("H/S D", cut.Markup);
            Assert.Contains("href=\"soshoe/Distribution\"", cut.Markup);
            Assert.Contains("href=\"soshoe/Generate/Distribution\"", cut.Markup);

            Assert.Contains("H/S F", cut.Markup);
            Assert.Contains("href=\"soshoe/Flow\"", cut.Markup);
            Assert.Contains("href=\"soshoe/Generate/Flow\"", cut.Markup);

            Assert.Contains("H/S S", cut.Markup);
            Assert.Contains("href=\"soshoe/Sequence\"", cut.Markup);
            Assert.Contains("href=\"soshoe/Generate/Sequence\"", cut.Markup);
        }

    }
}
