public enum CardOrder
{
    Coleoptera,
    Diptera,
    Hymenoptera,
    Mantodea,
    Lepidoptera,
    Araneae,
    Odonata,
    Siphonaptera,
    Orthoptera,
    Other,
}

public class CardOrderInfo
{
    public static string CardOrderString(CardOrder order)
    {
        switch (order)
        {
            case CardOrder.Coleoptera:
                return "Coleoptera";
            case CardOrder.Diptera:
                return "Diptera";
            case CardOrder.Hymenoptera:
                return "Hymenoptera";
            case CardOrder.Mantodea:
                return "Mantodea";
            case CardOrder.Lepidoptera:
                return "Lepidoptera";
            case CardOrder.Araneae:
                return "Araneae";
            case CardOrder.Odonata:
                return "Odonatra";
            case CardOrder.Siphonaptera:
                return "Siphonaptera";
            case CardOrder.Orthoptera:
                return "Orthoptera";
            case CardOrder.Other:
                return "Other";
        }
        return "Other";
    }
}