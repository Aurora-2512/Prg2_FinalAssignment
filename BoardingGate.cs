using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightInfoDisplay
{
    internal class BoardingGate
    {
        private string gateName;
        private bool supportCFFT;
        private bool supportDDJB;
        private bool supportLWTT;
        private Flight f;

        public BoardingGate(string gateName, bool supportDDJB, bool supportCFFT, bool supportLWTT)
        {
            this.GateName = gateName;
            this.supportCFFT = supportCFFT;
            this.supportDDJB = supportDDJB;
            this.supportLWTT = supportLWTT;
            
        }

        public string GateName { get => gateName; set => gateName = value; }
        public bool SupportCFFT { get => supportCFFT; set => supportCFFT = value; }
        public bool SupportDDJB { get => supportDDJB; set => supportDDJB = value; }
        public bool SupportLWTT { get => supportLWTT; set => supportLWTT = value; }
        internal Flight F { get => f; set => f = value; }

        public double calculateFees()
        {
            return 0;
        }

        public override string ToString()
        {
            return $"{GateName,-10} {supportDDJB,-8} {supportCFFT,-8} {supportLWTT,-8}";
        }
    }

}
