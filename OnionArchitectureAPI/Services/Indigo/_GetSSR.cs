using Bookingmanager_;
using DomainLayer.Model;
using Indigo;
using IndigoBookingManager_;
using Newtonsoft.Json;
using Utility;

namespace OnionArchitectureAPI.Services.Indigo
{
    public class _GetSSR
    {
        Logs logs = new Logs();


        public async Task<List<IndigoBookingManager_.GetSeatAvailabilityResponse>> GetseatAvailability(string Signature, AirAsiaTripResponceModel AirAsiaTripResponceobj, string _AirlineWay = "")
        {
            IndigoBookingManager_.GetSeatAvailabilityRequest _getseatAvailabilityRequest = new IndigoBookingManager_.GetSeatAvailabilityRequest();
            IndigoBookingManager_.GetSeatAvailabilityResponse _getSeatAvailabilityResponse = new IndigoBookingManager_.GetSeatAvailabilityResponse();

            _getseatAvailabilityRequest.Signature = Signature;
            _getseatAvailabilityRequest.ContractVersion = 452;

            IndigoBookingManager_.SeatAvailabilityRequest _seatRequest = new IndigoBookingManager_.SeatAvailabilityRequest();
            List<IndigoBookingManager_.GetSeatAvailabilityResponse> SeatGroup = new List<IndigoBookingManager_.GetSeatAvailabilityResponse>();
            if (AirAsiaTripResponceobj.journeys.Count > 0)
            {
                for (int a = 0; a < AirAsiaTripResponceobj.journeys.Count; a++)
                {
                    for (int i = 0; i < AirAsiaTripResponceobj.journeys[a].segments.Count; i++)
                    {
                        _seatRequest = new IndigoBookingManager_.SeatAvailabilityRequest();
                        _seatRequest.STDSpecified = true;
                        _seatRequest.STD = AirAsiaTripResponceobj.journeys[a].segments[i].designator.departure;
                        _seatRequest.DepartureStation = AirAsiaTripResponceobj.journeys[a].segments[i].designator.origin;
                        _seatRequest.ArrivalStation = AirAsiaTripResponceobj.journeys[a].segments[i].designator.destination;
                        _seatRequest.IncludeSeatFees = true;
                        _seatRequest.IncludeSeatFeesSpecified = true;
                        _seatRequest.SeatAssignmentModeSpecified = true;
                        _seatRequest.SeatAssignmentMode = IndigoBookingManager_.SeatAssignmentMode.PreSeatAssignment;
                        _seatRequest.FlightNumber = AirAsiaTripResponceobj.journeys[a].segments[i].identifier.identifier;
                        _seatRequest.OverrideSTDSpecified = true;
                        _seatRequest.OverrideSTD = AirAsiaTripResponceobj.journeys[a].segments[i].designator.departure;
                        _seatRequest.CarrierCode = AirAsiaTripResponceobj.journeys[a].segments[i].identifier.carrierCode;
                        _seatRequest.EnforceSeatGroupRestrictions = false;
                        _getseatAvailabilityRequest.SeatAvailabilityRequest = _seatRequest;
                        _getapi _obj = new _getapi();
                        _getSeatAvailabilityResponse = await _obj.GetseatAvailability(_getseatAvailabilityRequest,a,i);
                        SeatGroup.Add(_getSeatAvailabilityResponse);

                    }
                }

            }

            string str1 = JsonConvert.SerializeObject(SeatGroup);
            if (_AirlineWay.ToLower() == "oneway")
            {
                logs.WriteLogs("Request: " + JsonConvert.SerializeObject(_getseatAvailabilityRequest) + "\n\n Response: " + JsonConvert.SerializeObject(SeatGroup), "GetSeatAvailability", "IndigoOneWay");
            }
            else
            {
                logs.WriteLogsR("Request: " + JsonConvert.SerializeObject(_getseatAvailabilityRequest) + "\n\n Response: " + JsonConvert.SerializeObject(SeatGroup), "GetSeatAvailability", "SameIndigoRT");
            }
            return SeatGroup;

        }


        public async Task<List<IndigoBookingManager_.GetSSRAvailabilityForBookingResponse>> GetSSRAvailabilityForBooking(string Signature, AirAsiaTripResponceModel passeengerlist, int TotalCount, string _AirlineWay = "")
        {
            List<IndigoBookingManager_.GetSSRAvailabilityForBookingResponse> SSRGroup = new List<IndigoBookingManager_.GetSSRAvailabilityForBookingResponse>();
            IndigoBookingManager_.GetSSRAvailabilityForBookingRequest _req = new IndigoBookingManager_.GetSSRAvailabilityForBookingRequest();
            IndigoBookingManager_.GetSSRAvailabilityForBookingResponse _res = new IndigoBookingManager_.GetSSRAvailabilityForBookingResponse();
            try
            {
                int segmentcount = 0;
                int journeyscount = passeengerlist.journeys.Count;
                _req.Signature = Signature;
                _req.ContractVersion = 456;
                IndigoBookingManager_.SSRAvailabilityForBookingRequest _SSRAvailabilityForBookingRequest = new IndigoBookingManager_.SSRAvailabilityForBookingRequest();

                for (int i = 0; i < journeyscount; i++)
                {
                    int segmentscount = passeengerlist.journeys[i].segments.Count;
                    _SSRAvailabilityForBookingRequest.SegmentKeyList = new IndigoBookingManager_.LegKey[segmentscount];
                    for (int j = 0; j < segmentscount; j++)
                    {
                        _SSRAvailabilityForBookingRequest.SegmentKeyList[j] = new IndigoBookingManager_.LegKey();
                        int legcount = passeengerlist.journeys[i].segments[j].legs.Count;
                        for (int n = 0; n < legcount; n++)
                        {
                            _SSRAvailabilityForBookingRequest.SegmentKeyList[j].CarrierCode = passeengerlist.journeys[i].segments[j].identifier.carrierCode;
                            _SSRAvailabilityForBookingRequest.SegmentKeyList[j].FlightNumber = passeengerlist.journeys[i].segments[j].identifier.identifier;
                            _SSRAvailabilityForBookingRequest.SegmentKeyList[j].DepartureDateSpecified = true;
                            //string strdate = Convert.ToDateTime(passengerdetails.departure).ToString("yyyy-MM-dd");
                            _SSRAvailabilityForBookingRequest.SegmentKeyList[j].DepartureDate = Convert.ToDateTime(passeengerlist.journeys[i].segments[j].designator.departure);//DateTime.ParseExact(strdate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            _SSRAvailabilityForBookingRequest.SegmentKeyList[j].ArrivalStation = passeengerlist.journeys[i].segments[j].designator.destination;
                            _SSRAvailabilityForBookingRequest.SegmentKeyList[j].DepartureStation = passeengerlist.journeys[i].segments[j].designator.origin;
                            segmentcount++;
                        }
                    }

                    _SSRAvailabilityForBookingRequest.PassengerNumberList = new short[Convert.ToInt16(TotalCount)];//new short[1];
                    int paxCount = _SSRAvailabilityForBookingRequest.PassengerNumberList.Length;//passeengerlist.passengerscount;
                    for (int a = 0; a < paxCount; a++)
                    {
                        if (a > 0)
                            continue;
                        _SSRAvailabilityForBookingRequest.PassengerNumberList[a] = Convert.ToInt16(a);
                    }
                    _SSRAvailabilityForBookingRequest.InventoryControlled = true;
                    _SSRAvailabilityForBookingRequest.InventoryControlledSpecified = true;
                    _SSRAvailabilityForBookingRequest.NonInventoryControlled = true;
                    _SSRAvailabilityForBookingRequest.NonInventoryControlledSpecified = true;
                    _SSRAvailabilityForBookingRequest.SeatDependent = true;
                    _SSRAvailabilityForBookingRequest.SeatDependentSpecified = true;
                    _SSRAvailabilityForBookingRequest.NonSeatDependent = true;
                    _SSRAvailabilityForBookingRequest.NonSeatDependentSpecified = true;
                    _SSRAvailabilityForBookingRequest.CurrencyCode = "INR";
                    _SSRAvailabilityForBookingRequest.SSRAvailabilityMode = IndigoBookingManager_.SSRAvailabilityMode.NonBundledSSRs;
                    _SSRAvailabilityForBookingRequest.SSRAvailabilityModeSpecified = true;
                    _req.SSRAvailabilityForBookingRequest = _SSRAvailabilityForBookingRequest;
                    _getapi _obj = new _getapi();
                    _res = await _obj.GetMealAvailabilityForBooking(_req);
                    SSRGroup.Add(_res);
                    string Str2 = JsonConvert.SerializeObject(SSRGroup);
                    if (_AirlineWay.ToLower() == "oneway")
                    {
                        logs.WriteLogsR("Request: " + JsonConvert.SerializeObject(_req) + "\n\n Response: " + JsonConvert.SerializeObject(_res), "GetSSRAvailabilityForBooking", "IndigoOneWay");
                    }
                    else
                    {
                        logs.WriteLogsR("Request: " + JsonConvert.SerializeObject(_req) + "\n\n Response: " + JsonConvert.SerializeObject(_res), "GetSSRAvailabilityForBooking", "SameIndigoRT");
                    }
                }

                
                return SSRGroup;

            }
            catch (Exception ex)
            {

            }
            return SSRGroup;
        }

    }



}

