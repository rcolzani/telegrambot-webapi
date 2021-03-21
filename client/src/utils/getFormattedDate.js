import moment from 'moment'

export default function getFormattedDate(dateToFormat) {
    if (!dateToFormat) {
        return "";
    }
    moment.locale('pt-br');
    const formattedDate = moment(dateToFormat).format('DD/MM/YYYY HH:mm:ss');
    return formattedDate;
}
