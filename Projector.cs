using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rv;
using System.Threading;

namespace SimpleMediaPlayer
{
    public class Projector
    {
        public string host;
        public int port = 4352;

        public int hours = -1;
        public string status = "?";
        public bool isOn = false;

        public bool connected = false;
        public bool available = false;

        public PJLinkConnection pjcon = null;

        public Command.Response last_reponse = Command.Response.SUCCESS;

        public Projector(string host)
        {
            this.host = host;
        }

        public PJLinkConnection connect(){
            if(pjcon == null){
                pjcon = new PJLinkConnection(this.host);
            }
            return pjcon;
        }

        public PowerCommand.PowerStatus getPower()
        {
            var pj = connect();
            PowerCommand cmd = new PowerCommand(PowerCommand.Power.QUERY);
            last_reponse = pj.sendCommand(cmd);
            isOn = false;
            status = "?";
            if (last_reponse == Command.Response.SUCCESS)
            {
                switch (cmd.Status)
                {
                    case PowerCommand.PowerStatus.OFF:
                        status = "OFF";
                        break;
                    case PowerCommand.PowerStatus.ON:
                        status = "ON";
                        isOn = true;
                        break;
                    default:
                        status = "?";
                        break;
                }
                return cmd.Status;
            }
            return PowerCommand.PowerStatus.UNKNOWN;
        }

        private PowerCommand.PowerStatus processPowerResult()
        {
            if (last_reponse == Command.Response.SUCCESS)
            {
                PowerCommand.PowerStatus ret = PowerCommand.PowerStatus.UNKNOWN;
                for (int i = 0; i < 60; i++)
                {
                    Thread.Sleep(500);
                    ret = getPower();
                    if (last_reponse != Command.Response.SUCCESS)
                    {
                        return PowerCommand.PowerStatus.UNKNOWN;
                    }
                    if (ret == PowerCommand.PowerStatus.ON)
                    {
                        return ret;
                    }
                }
                return ret;
            }
            return PowerCommand.PowerStatus.UNKNOWN;
        }

        public PowerCommand.PowerStatus turnOn()
        {
            var pj = connect();
            PowerCommand cmd = new PowerCommand(PowerCommand.Power.ON);
            last_reponse = pj.sendCommand(cmd);
            return processPowerResult();
        }

        public PowerCommand.PowerStatus turnOff()
        {
            var pj = connect();
            PowerCommand cmd = new PowerCommand(PowerCommand.Power.OFF);
            last_reponse = pj.sendCommand(cmd);
            return processPowerResult();
        }
    }

}
