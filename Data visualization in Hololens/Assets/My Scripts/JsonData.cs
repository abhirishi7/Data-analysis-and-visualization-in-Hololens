using UnityEngine;
using System.Collections;



namespace Assets.My_Scripts
{
    /*=======================================================================================================================
     * Common  
     *======================================================================================================================*/
    [System.Serializable]
    public class TimelineAll
    {
        public long Jan;
        public long Feb;
        public long Mar;
        public long Apr;
        public long May;
        public long Jun;
        public long Jul;
        public long Aug;
        public long Sep;
        public long Oct;
        public long Nov;
        public long Dec;

        public long Q1;
        public long Q2;
        public long Q3;
        public long Q4;

        public long Year;
    }//class : TimelineAll


    [System.Serializable]
    public class TopVendorDataArray
    {
        public TopVendorData[] topVen;
    }//class : TopVendorDataArray

    [System.Serializable]
    public class TopVendorData
    {
        public int id;
        public int[] vid15;
        public int[] vid16;
    }//class : TopVendorData

    [System.Serializable]
    public class DivisionTopBrandDataArray
    {
        public DivisionTopBrandData[] divTop;
    }//class : DivisionTopBrandDataArray

    [System.Serializable]
    public class DivisionTopBrandData
    {
        public int did;
        public TopBrandData[] ven;
    }//class : DivisionTopBrandData

    [System.Serializable]
    public class TopBrandData
    {
        public int id;
        public int[] pid15;
        public int[] pid16;
    }//class : TopBrandData


    /*=======================================================================================================================
     * Overview Data Storage  
     *======================================================================================================================*/

    [System.Serializable]
    public class OverviewDataArray
    {
        public OverviewData[] overview;
    }//class : OverviewDataArray

    [System.Serializable]
    public class OverviewData
    {
        public string DivisionName;
        public string DivisionDesc;
        public long NetSalesValue;
        public long NetSalesTTL;
        public int NetSalesGrowth;
        public int DirectValue;
        public int IndirectValue;
        public int TotalGrowthContribution;
    }//class : OverviewData


   
        
    /*=======================================================================================================================
     * Net Sales And Sell Out Data Storage  
     *======================================================================================================================*/

    [System.Serializable]
    public class EntityDataArray
    {
        public EntityData[] entity;
    }//class : EntityDataArray

    [System.Serializable]
    public class EntityData
    {
        public string EntityName;
        public string EntityDesc;
        public TimelineAll Entity15Value;
        public TimelineAll Entity16Value;
        public long EntityTTL15;
        public long EntityTTL16;
        public int EntityGrowth;
    }//class : EntityData



    /*=======================================================================================================================
     *  E-Commerce Data Storage
     *======================================================================================================================*/

    [System.Serializable]
    public class ECommerceDataArray
    {
        public ECommerceData[] ecommerce;
    }//class : ECommerceDataArray

    [System.Serializable]
    public class ECommerceData
    {
        public string DivisionName;
        public string EcommmerceName;
        public long Ecommerce15Value;
        public long Ecommerce16Value;
        public long ECommerce15TTL;
        public long ECommerce16TTL;
        public int EcommerceGrowth;
        public float ValueToShow = 0f;
    }//class : ECommerceData
    

    
    /*=======================================================================================================================
     *  Channel Data Storage
     *======================================================================================================================*/
     
    [System.Serializable]
    public class ChannelDataArray
    {
        public ChannelData[] channel;
    }//class : ChannelDataArray

    [System.Serializable]
    public class ChannelData
    {
        public string DivisionName;
        public string ChannelName;
        public TimelineAll Channel15Value;
        public TimelineAll Channel16Value;
        public long Channel15TTL;
        public long Channel16TTL;
        public int ChannelGrowth;
    }//class : ChannelData

   

    
    /*=======================================================================================================================
     *  Vendor Data Storage
     *======================================================================================================================*/

    [System.Serializable]
    public class VendorDataArray
    {
        public VendorDivData[] venDiv;
    }//class : VendorDataArray

    [System.Serializable]
    public class VendorDivData
    {
        public VendorData[] vendor;
    }//class : VendorDivData

    [System.Serializable]
    public class VendorData
    {
        public string VendorName;
        public string VendorDesc;
        public TimelineAll Vendor15Value;
        public TimelineAll Vendor16Value;
        public long Vendor15TTL;
        public long Vendor16TTL;
        public int VendorGrowth;
    }//class : VendorData

}//namespace
