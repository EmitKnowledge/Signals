using App.Clients.Processes;
using Ganss.XSS;
using Signals.Core.Processes;
using Signals.Core.Processes.Api;
using Signals.Core.Processing.Input;
using Signals.Core.Processing.Results;
using System.Collections.Generic;

namespace Signals.Clients.WebApi.ApiProcesses
{
    public class TestProcess : ApiProcess<PlasmaRequestDto, MethodResult<PlasmaRequestDto>>
    {
        public override MethodResult<PlasmaRequestDto> Auth(PlasmaRequestDto dto)
        {
            return Ok();
        }

        public override MethodResult<PlasmaRequestDto> Validate(PlasmaRequestDto dto)
        {
            return Ok();
        }

        public override MethodResult<PlasmaRequestDto> Handle(PlasmaRequestDto dto)
        {
            return dto;
        }
    }

    public class PlasmaRequestDto : IDtoData
	{
        public List<PlasmaDto> PlasmaDtos = new List<PlasmaDto>();

        public void Sanitize(HtmlSanitizer sanitizer)
        {
        }
    }

    public class PlasmaDto
    {
        public string CutSpeed { get; set; }
        public string Kerf { get; set; }
        public string PierceTime { get; set; }
        public string CreepTime { get; set; }
        public string CreepSpeed { get; set; }
        public string TransferHeight { get; set; }
        public string CutHeight { get; set; }
        public string PierceHeight { get; set; }
        public string SetArcVoltage { get; set; }
        public string ProcessCurrent { get; set; }
        public string Technology { get; set; }
        public string Vendor { get; set; }
        public string Nozzle { get; set; }
        public string Electrode { get; set; }
        public string WaterTube { get; set; }
        public string Shield { get; set; }
        public string ShieldRetainingCap { get; set; }
        public string SwirlRing { get; set; }
        public string TorchType { get; set; }
        public string MaterialThickness { get; set; }
        public string MaterialType { get; set; }
        public string PlasmaShieldGases { get; set; }
        public string Process { get; set; }
        public string AirFlowHot { get; set; }
        public string AirFlowCold { get; set; }
	}
}
